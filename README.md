# Webcam server

## About the project

This repo contains code for an a webserver that exposes the state of your web cam on Windows. I wrote it to provide status information for my [ESP32 web cam 'busy' light](https://github.com/danbowker/esp-cam-light).

## Installation

Once you've built the project, you can either run directly or install it as a Windows service. To install as a Windows service open an administrator Powershell prompt:

```ps
new-service webcam -BinaryPathName "[path to webcam.exe]" -DisplayName "Web Cam Status" -StartupType Automatic -Credential [your local windows username]
```

Your username probably needs to be fully qualified with local machine or domain name. The service needs to run with the same user account that you use to sign in to Windows because it reads from HKCU in the registry to get the current user web cam status.
