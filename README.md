# winui-kanban-examples

This repository contains different samples demonstrating about [WinUI Kanban](https://help.syncfusion.com/winui/kanban/getting-started) control.

## Syncfusion controls

This project used the following Syncfusion control(s):
* [SfKanban](https://www.syncfusion.com/winui-controls/kanban)

## Supported platforms

* Windows 11 and Windows 10 version 1809 or higher, using [Windows UI Library (WinUI) 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/).

## Requirements to run the sample

* [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
* [Windows App SDK 1.2 extension](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/stable-channel#version-124-122302174)
* .NET 6.0/.Net 7.0/.Net 8.0/.Net 9.0

## How to run the sample

1. Clone the sample and open it in Visual Studio 2022 preview.
   
   *Note: If you download the sample using the "Download ZIP" option, right-click it, select Properties, and then select Unblock.*

2. Register your license key in the App.cs file as demonstrated in the following code.

		public App()
		{
			//Register Syncfusion license
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR LICENSE KEY");
		
			InitializeComponent();
		}
    Refer to this [link](https://help.syncfusion.com/winui/licensing/overview) for more details.

3. Clean and build the application.

4. Run the application.

## License

Syncfusion has no liability for any damage or consequence that may arise from using or viewing the samples. The samples are for demonstrative purposes. If you choose to use or access the samples, you agree to not hold Syncfusion liable, in any form, for any damage related to use, for accessing, or viewing the samples. By accessing, viewing, or seeing the samples, you acknowledge and agree Syncfusion’s samples will not allow you seek injunctive relief in any form for any claim related to the sample. If you do not agree to this, do not view, access, utilize, or otherwise do anything with Syncfusion’s samples.