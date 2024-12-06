import threading
import socket
import time
import cv2
import mediapipe as mp
import numpy as np
from deepface import DeepFace
from keras.models import load_model
from tensorflow.keras.utils import img_to_array
import struct

# Update the model path
emotion_classifier = load_model(r"C:\Users\sohai\Downloads\Phase3-HCi-main\Phase3-HCi-main\Phase3-HCi-main\model.h5")
    
# Update the Haar Cascade path
face_cascade = cv2.CascadeClassifier(r"C:\Users\sohai\Downloads\Phase3-HCi-main\Phase3-HCi-main\Phase3-HCi-main\haarcascade_frontalface_default.xml")

emotion_labels = ['Angry', 'Disgust', 'Fear', 'Happy', 'Neutral', 'Sad', 'Surprise']

reference_img1 = cv2.imread(r'C:\Users\sohai\Downloads\Phase3-HCi-main\Phase3-HCi-main\Phase3-HCi-main\Img\bido.jpg')

recognized_text = ""
recognized_time = 0
current_emotion = ""
conn = None

def check_face(captured_image_path):
    global recognized_text, recognized_time
    try:
        frame = cv2.imread(captured_image_path)
        if DeepFace.verify(frame, reference_img1.copy(), enforce_detection=False)['verified']:
            recognized_text = "Abdelrahman"
        else:
            recognized_text = ""
        if recognized_text:
            recognized_time = time.time()
    except ValueError:
        recognized_text = ""

def check_face(captured_image_path):
    global recognized_text, recognized_time
    try:
        frame = cv2.imread(captured_image_path)
        if DeepFace.verify(frame, reference_img1.copy(), enforce_detection=False)['verified']:
            recognized_text = "Abdelrahman"
        else:
            recognized_text = ""
        if recognized_text:
            recognized_time = time.time()
    except ValueError:
        recognized_text = ""

def recognize_face_and_detect_emotion_and_capture_hand_coordinates(sock):
    mp_drawing = mp.solutions.drawing_utils
    mp_hands = mp.solutions.hands
    hands = mp_hands.Hands(min_detection_confidence=0.8, min_tracking_confidence=0.5)
    cap = cv2.VideoCapture(1)  # Ensure the correct camera index

    while True:
        ret, frame = cap.read()
        if not ret:
            print("Camera not accessible")
            continue

        frame_height, frame_width, _ = frame.shape
        frame = cv2.flip(frame, 1)

        # Convert to RGB for Mediapipe
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = hands.process(image)

        # Face detection and emotion recognition
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray)
        for (x, y, w, h) in faces:
            cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 255), 2)
            roi_gray = gray[y:y + h, x:x + w]
            roi_gray = cv2.resize(roi_gray, (48, 48), interpolation=cv2.INTER_AREA)
            if np.sum([roi_gray]) != 0:
                roi = roi_gray.astype('float') / 255.0
                roi = img_to_array(roi)
                roi = np.expand_dims(roi, axis=0)
                prediction = emotion_classifier.predict(roi)[0]
                current_emotion = emotion_labels[prediction.argmax()]
                cv2.putText(frame, current_emotion, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

        # Hand detection
        hand_coordinates = []
        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                mp_drawing.draw_landmarks(frame, hand_landmarks, mp_hands.HAND_CONNECTIONS)
                x = int(hand_landmarks.landmark[8].x * frame_width)
                y = int(hand_landmarks.landmark[8].y * frame_height)
                hand_coordinates.append(f"{x},{y}")

        # Prepare data to send (hand coordinates + emotion data)
        if hand_coordinates:
            try:
                # Send both emotion and hand data over the socket
                coordinates_data = ";".join(hand_coordinates)
                data_to_send = f"{coordinates_data} {current_emotion}"
                sock.sendall(data_to_send.encode('utf-8'))
                print(f"Sent: {data_to_send}")
            except Exception as e:
                print(f"Error sending coordinates/emotions: {e}")

        cv2.imshow('Emotion and Hand Tracking', frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    cap.release()
    cv2.destroyAllWindows()


def socket_listener():
    global conn
    mySocket = socket.socket()
    mySocket.bind(("localhost", 5050))
    mySocket.listen(5)
    print("Waiting for connection...")
    conn, addr = mySocket.accept()
    print("Device connected")
    recognize_face_and_detect_emotion_and_capture_hand_coordinates(conn)
    conn.close()
    mySocket.close()


thread = threading.Thread(target=socket_listener)
thread.start()
thread.join()
