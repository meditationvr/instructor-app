# Meditation VR - Instructor App

This repository contains a Android/Desktop Unity project that enables instructors to create virtual meditation rooms. On that note, this app uses information from the students Muse EEG device (Muse 2016) to catch real-time brainwaves during the meditation process. This data is used to generate real-time ML models and guide the student during meditation sessions.

## Getting Started

* To run this api just follow the steps on [compose](https://github.com/MeditationVR/compose)

## Important links and tips

* Setting up Facebook Login
    * https://developers.facebook.com/docs/unity/gettingstarted

* How to generate a valid Android key for your app
    * Add OpenSSL and JDK (keystore) to the System Path
        * `C:\Program Files\Java\jdk1.8.0_201\bin`
        * `C:\Program Files\openssl-0.9.8k_X64\bin`
            * https://code.google.com/archive/p/openssl-for-windows/downloads
    * `cd C:\Program Files\Java\jdk1.8.0_201\bin`
    * `keytool -exportcert -alias androiddebugkey -keystore "C:\Users\Daniel\.android\debug.keystore" | "C:\Program Files\openssl-0.9.8k_X64\bin\openssl" sha1 -binary | "C:\Program Files\openssl-0.9.8k_X64\bin\openssl" base64`