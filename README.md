# Unity_Gamification_Assets

Useful template assets for unity

# Assets

# WebCam
Access WebCam from Unity and render it in RawImage, can toggle between available webcams. (Captures only Photo)

ScreenShots

![WebCam](ScreenShots/webcam_1.png)
![WebCam](ScreenShots/webcam_2.png)

# AutoCompleteDropDown 
A DropDown with Inputfield to filter the dropdown values

ScreenShots

![AutoCompleteDropDown](ScreenShots/AutoComplete_1.png)
![AutoCompleteDropDown](ScreenShots/AutoComplete_2.png)
![AutoCompleteDropDown](ScreenShots/AutoComplete_3.png)
![AutoCompleteDropDown](ScreenShots/AutoComplete_4.png)

# DailyRewards
DailyRewards provides a rewards daliy basis

ScreenShots

![DailyRewards](ScreenShots/DailyReward_1.png)
![DailyRewards](ScreenShots/DailyReward_2.png)
![DailyRewards](ScreenShots/DailyReward_3.png)
![DailyRewards](ScreenShots/DailyReward_4.png)

# DatePicker 
Date picker with two design

ScreenShots

![DatePicker](ScreenShots/DatePicker_1.png)
![DatePicker](ScreenShots/DatePicker_2.png)
![DatePicker](ScreenShots/DatePicker_3.png)
![DatePicker](ScreenShots/DatePicker_4.png)

# EncryptedPlayerPrefs
Playerprefs values are encrypted 

# Facebook 
Facebook helper functions

# FTPManager
Helper function to connect FTP and downloads a file within Unity

# Google
# Achievements
Achievements helper functions 

# Leaderboard
Leaderboard helper functions 

# Save Score
Save Score helper functions 

# HourlyRewards
HourlyRewards provides a rewards hourly basis 

ScreenShots

![HourlyRewards](ScreenShots/Hourly_Reward_1.png)
![HourlyRewards](ScreenShots/Hourly_Reward_2.png)
![HourlyRewards](ScreenShots/Hourly_Reward_3.png)

# NetworkCallback
Network callback asset provides callback action on api request.

# Pagination
Pagination asset provides images to move left or right with dots indicator.

ScreenShots

![Pagination](ScreenShots/pagination_1.png)
![Pagination](ScreenShots/pagination_2.png)

# PanZoomUIImage
PanZoomUIImage asset helps image to pan, zoom with both mouse and touch.

ScreenShots

![PanZoomUIImage](ScreenShots/pan_zoom_ui_1.png)
![PanZoomUIImage](ScreenShots/pan_zoom_ui_2.png)

# ParallaxScrollEffect
ParallaxScrollEffect asset helps images to move either vertically or horizontally.

ScreenShots

![ParallaxScrollEffect](ScreenShots/parallax_scroll_1.png)
![ParallaxScrollEffect](ScreenShots/parallax_scroll_2.png)
![ParallaxScrollEffect](ScreenShots/parallax_scroll_3.png)

# PDFViewer
PDFViewer helps to view PDF inside unity, its converts pages into multiple images and renders into image. (Using Ghostscript Lib)

# PlayingCards
# CardFlip Animation
Playing card flip animation using DoTween Animation

ScreenShots

![CardFlip](ScreenShots/card_flip_anim_2.png)
![CardFlip](ScreenShots/card_flip_anim_4.png)
![CardFlip](ScreenShots/card_flip_anim_6.png)

# CardLayout
Card layout is a group layout helps cards to align in 3 different ways. (Best suits for Solitaire)

ScreenShots

![CardLayout](ScreenShots/CardsLayout_1.png)
![CardLayout](ScreenShots/CardsLayout_2.png)

# DragNDrop
Drag and Drop images

![DragNDrop](ScreenShots/DragNDrop_1.png)
![DragNDrop](ScreenShots/DragNDrop_2.png)
![DragNDrop](ScreenShots/DragNDrop_3.png)

ScreenShots

# RummyLayout
Rummy layout is a group layout helps cards to align in groups and also creates new group. (Best suits for Rummy)

ScreenShots

![RummyLayout](ScreenShots/RummyLayout_1.png)
![RummyLayout](ScreenShots/RummyLayout_2.png)
![RummyLayout](ScreenShots/RummyLayout_3.png)

# PoolManager
Asset helps to reuse poolable item rather destory and instantiate

ScreenShots

![PoolManager](ScreenShots/PoolManager_1.png)
![PoolManager](ScreenShots/PoolManager_2.png)

# QRCode
QRCode helps to create and read a QRCode with in unity. (Using Zxing Lib)

ScreenShots

![QRCode](ScreenShots/qrcode_1.png)
![QRCode](ScreenShots/qrcode_2.png)

# RectScreenShot
RectScreenShot helps to capture a recttransform

ScreenShots

![RectScreenShot](ScreenShots/RectScreenShot_1.PNG)
![RectScreenShot](ScreenShots/ScreenShot.png)

# SceneLoader
SceneLoader asset provides loading screen while chaning scenes with 3 ways - Instant, Press any key and Press Button

ScreenShots

![SceneLoader](ScreenShots/scene_loader_1.png)
![SceneLoader](ScreenShots/scene_loader_2.png)
![SceneLoader](ScreenShots/scene_loader_3.png)

# ScrollableText
ScrollableText useful for showing huge text to autoscroll. (Like in credits screen, Privacy Policy etc)

ScreenShots

![ScrollableText](ScreenShots/scrollable_Text_1.png)
![ScrollableText](ScreenShots/scrollable_Text_2.png)

# SimpleCSV
SimpleCSV asset helps to read and write a CSV file within Unity

ScreenShots

![SimpleCSV](ScreenShots/simple_csv_1.png)
![SimpleCSV](ScreenShots/simple_csv_2.png)

# Singleton
An Abstract Singleton class for both Monobehaviour and Non Monobehaviour class.

# SMTPManager
Helper function to connect SMTP and send Email within Unity

# SpinWheel
SpinWheel provides a wheel with rewards 

# UIFlipOrientation
UIFlipOrientation asset helps the group layout to chage its orientation based on device orientation.

ScreenShots

![UIFlipOrientation](ScreenShots/UI_Flip_Orientation_1.png)
![UIFlipOrientation](ScreenShots/UI_Flip_Orientation_2.png)

# WrapLayout
WrapLayout asset helps the group layout to align all text elemets to wrap within the group layout

ScreenShots

![WrapLayout](ScreenShots/WrapLayout.png)


# LoggerUtils
Logger utility for logging custom error or log messages to console and file.

```C#
public enum LogOutput : byte
{
    Unity = 0,
    Console = 1,
    FileOnly = 2
}

private void Awake()
{
#if UNITY_EDITOR
    LoggerUtils.ToogleLogOnDevice(true);
    LoggerUtils.SetLogProfile(LogProfile.UnityDebug);
#endif

    LoggerUtils.LogWarning("FirstLog");
}

```

# Playfab
PlayFab is a complete backend platform for live games with managed game services, real-time analytics, and LiveOps. PlayFab enables developers to use the intelligent cloud to build and operate games, analyze gaming data and improve overall gaming experiences.

# UserDeviceInfo
* Device Location
* Device Unique Id
* Device Mac Address
