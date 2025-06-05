
# Wishlist

## Functionality
- Add filters to TaskFilters

## UI

## Documentation & External Resources

## Known Issues
- 3.0.3 - The automatic update functionality is still in its early stages and may not work perfectly on all systems. If you encounter issues, please report them on the GitHub repository.
- 4.3.2 - STA Threading issues, especially around automatic updates. This may cause the application to hang or crash in some cases. A fix is in progress.

# CalliAPI Patch Log

## 4.5 - Major Refactoring, Bug Fixes, and Improvements
- Simplified "using" statements throughout the codebase to improve readability and maintainability. This change will help streamline the code and make it easier to understand and modify in the future.
- Updated DLL Import in Main to use LibraryImport instead, which is a more modern and efficient way to handle DLL imports in .NET. This change will improve performance and reduce potential issues related to DLL loading.
- Simplified collection initialization in the codebase to enhance performance and reduce memory overhead. This change will help optimize the application's performance, especially when dealing with large datasets or collections.
- Removed the Reports MenuStrip item to ensure that the Custom Report Builder is the primary way to generate reports. This change will help streamline the user experience and focus on the Custom Report Builder as the main reporting tool.
- Added a new "About" menu item to the Help MenuStrip, providing users with information about the application, its version, and how to get support. This will enhance the user experience by making it easier to find help and information about CalliAPI.
- Added plenty of new comments throughout the codebase to improve documentation and make it easier for developers to understand the code. This change will help future developers quickly grasp the purpose and functionality of different parts of the code, making maintenance and updates easier.
- Added XML documentation to the codebase to provide detailed explanations of classes, methods, and properties. This will enhance the overall documentation quality and make it easier for developers to understand how to use and extend the application.
- Simplified the initialization of variables throughout the codebase to improve readability and maintainability. This change will help make the code cleaner and easier to understand, reducing potential confusion for developers working on the project, and aligns with best practices.
- Removed multiple unneeded functions and variables from the codebase to streamline the application and improve performance. This change will help reduce complexity and make the codebase cleaner and more efficient.
- Consolidated the ClioService in order to simplify the codebase and improve maintainability. This change will help reduce redundancy and make it easier to manage the ClioService functionality, ensuring that all related code is in one place.
- Renamed multiple controls to use PascalCase, which is a common naming convention in C#. This change will improve code consistency and readability, making it easier for developers to understand the purpose of different controls and components.
- Added XML documentation to the Models in the codebase to provide detailed explanations of the data structures used in the application. This will enhance the overall documentation quality and make it easier for developers to understand how to use and extend the Models.

### 4.4.2 - Minor Improvements
- Added new fonts to the program, making it more visually appealing and easier to read.
- Added FRC and All Practice Area buttons to the Custom Report Builder, allowing users to quickly generate reports for specific practice areas.
- Rearranged the Custom Report Builder form to make it more user-friendly and intuitive.

### 4.4.1 - Added a Second Progress Bar for Spinning Up!
- Now we can see the progress that CalliAPI makes in the spin-up phase (where she checks how many pages to expect), which gives us more reliable UI indication and prevents surprises.

## 4.4 - Cleaned Custom Report Builder Form, Updated UI, and Added New Filters!
- Finally fixed the progress bar in the Custom Report Builder Form to provide an accurate estimate of page count before it begins to update. Now it won't be a surprise how long the program takes to complete a report or how many pages are needed! All of the 4.3 bug fixes are included as well for a polished 4.4!

### 4.3.4 - Bug Fixes and Improvements
- Added filtering by no active tasks in the Custom Report Builder! This enhancement allows users to filter reports based on whether there are active tasks associated with matters, making it easier to identify matters that may require attention or follow-up.

### 4.3.3 - Added ability to filter by Practice Area in Custom Report Builder!
This will save so much time in execution, especially as our dataset scales up! The ability to filter by Practice Area in the Custom Report Builder is a significant enhancement that will allow users to generate more targeted and relevant reports based on their specific practice areas. This feature will greatly improve the usability of the report builder and make it easier to analyze data related to different practice areas.

### 4.3.2 - Bug Fixes and Improvements
- Added the ability to search for SQL Views in the Custom Report Builder! This enhancement allows users to filter and generate reports based on specific views, making it easier to analyze data in a more structured way.

### 4.3.1 - Bug Fixes and Improvements
- Added parallelization to the Unworked 713 Search! Should help greatly increase the speed of the search.
- Updated the retryAfter Policy to include longer delays to allow for more respect from the Clio API.

## 4.3 - Bug Fixes and Improvements
Finally fixed the auto-update bug that arose in 4.2.2! This should resolve any issues users were experiencing with the auto-update feature, ensuring a smoother update process. Additionally, we have made some improvements to the overall performance and stability of the application.

### 4.2.2 - Bug Fixes and Improvements
- Removed extra logging in the Release version, to allow for better performance and cleaner logs. This should improve the overall user experience by reducing unnecessary log clutter and focusing on essential information.
- The logs are still visible when run in Debug mode, allowing developers to troubleshoot and monitor the application effectively during development.
- Attempting to fix an auto-update bug that caused the application to crash when trying to update. This should resolve any issues users were experiencing with the auto-update feature, ensuring a smoother update process.

### 4.2.1 - Bug Fixes and Improvements
- Error handling for the ReportForm when the xlsx file is already locked has been added. This will prevent crashes and ensure that users can handle file access issues gracefully.
- The 7 & 13 custom report was not working properly, so it has been fixed. This should resolve any issues users were experiencing with this specific report.

### 4.2 - Reporting Updates!
Custom Fields now appear in alphabetical order on the ReportForm, to make it easier to filter. We also have individual filters for each column! This will greatly improve the usability of the report builder and make it easier to generate reports tailored to specific needs. 

### 4.1 - Custom Report Builder Enhancements and Bug Fixes
We finally have friendly names for the Custom Fields in Clio!!! This means that when you use the custom report builder, the custom fields will now display their friendly names instead of their IDs, making it much easier to understand and use the report builder effectively. This enhancement significantly improves the usability of the custom report builder and makes it easier to generate reports tailored to specific needs.

#### 4.0.1 - Bug Fixes
Pointed the CLIO_CLIENT_SECRET to the proper place, so now the custom report builder should work properly with Clio queries!

## 4.0 - Custom Report Builder Enhancements and Bug Fixes; SQL Support!!!
We can now see into our SQL server with the custom report builder! This is a huge step forward in making CalliAPI more versatile and powerful. The custom report builder now supports SQL queries, allowing users to generate reports based on SQL data directly within the application. This enhancement will significantly improve the usability of the report builder and make it easier to generate reports tailored to specific needs.

### 3.2.1 - Splash Screen!
Also checks for updates on startup, so you can always be sure you're running the latest version of CalliAPI!

#### 3.1.2 - Added custom fields to custom report builder!
Also, finally fixed that really anying flickering progress bar issue (I hope)! The custom report builder now supports custom fields, allowing for more flexibility in report generation. This is a significant improvement that will enhance the usability of the report builder and make it easier to generate reports tailored to specific needs!
Finally, this patch also gives us the Page {X} of {Y} label back, which I missed dearly (due to a visual bug)!

#### 3.1.1 - Small bug fixes
Fixed the progress bar issue where it was not displaying the percentage correctly. This should now show the correct progress during report generation! Additionally, the CreatedSince dateTime picker should function properly in the ReportBuilder!

### 3.1 - CUSTOM REPORT BUILDER!!!!
CalliAPI has a custom report builder now! Adding deserialization into the Matter Parser for extra "nice".

#### 3.0.4 - Clio App Fixes
Added a field for the user to enter their Clio App secret key, allowing for more secure authentication. This change is crucial for ensuring that the CalliAPI can access Clio data without exposing sensitive information in the codebase.

#### 3.0.3 - Custom Field Adds!
Added 13584798 - Emergency Case Type! Doing this required a large rework to how we process Matters. Actual translation of picklist items will come soon!

#### 3.0.2 - Added Automatic updates
From 3.0.2 onwards, CalliAPI should also allow for automatic updates on each run!

## 3.0 - Restructure AuthService + GitHub Secret Support
AuthService is now done UNDERNEATH ClioService, and our Client secret is now stored safely in GitHub. The old Clio App client is now DELETED, so the new one is the only one that can be used to access CalliAPI. This means there's no risk that attackers will scrounge up the old Client secret, as it is deactivated.
This change is HUGE, as it allows us to securely store our secrets in GitHub, and it also simplifies the authentication process by making it more modular and easier to maintain.


### 2.1 - Updated download page and implemented auto-update functionality!
Plus, added raw image storage so we can easily update the logo and other images in the future! This is a big step forward in making CalliAPI more user-friendly and easier to maintain. The auto-update functionality will ensure that users always have the latest version without needing to manually download updates.

## 2.0 - Finally, automatic hosting on GitHub pages is live!
CalliAPI is now hosted on GitHub Pages, with a custom index page for easy installation! This is a huge step forward in making CalliAPI accessible to everyone at Amourgis & Associates, and it should make future updates much easier to manage!
[x] Wishlist item: Add a zip to the docs folder so that users can fallback on the zip completed!

#### 1.9.5 - UI tweaks and Matter Descriptions!
Now we can actually cancel the authorization dialog, instead of being stuck in limbo! Also, the Matter Descriptions are now included in the Matter searches, so we can retrieve them too!

### 1.8.0 - Prepping framework for a new way to install and landing pages!
Now CalliAPI has a more permanent home inside GitHub Pages with a custom-made index page for installing from!
(Don't ask about 1.5 and 1.6 and 1.7, it was all suffering with MSBuild, but now I know more I suppose!)

### 1.4.0 - Glow up in preparation for version 2!
A new UI, a new icon, and a new way of installing gives CalliAPI a lot more staying power! Couple that with the new Publish.html page and we have a way more user-friendly experience!

### 1.3.0 - Added VersionHelper and fixed some bugs!
Added a VersionHelper to help with maintaining version control. The ClickOnce version should now be the same as the version in the .csproj file, so we should be pretty good to go with updates and version control moving forward!

#### 1.2.1 - Centralized version numbering, expanded filters!
Added display_name to the fields we can parse from Clio into Matters, so now we have our own ideal way of sorting and identifying clients! It also gives us the ability to now FILTER by display_name, which is a huge win!

### 1.2.0 - Added UI implementation and KEYWORD SUPPORT!!!
I'm so pleased with this interface! The keyword inclusion is a gamechanger, and I'll continue moving reports into this TreeView UI so as to provide easy and clear definitions of terms!

### 1.1.0 - Wishlist (roadmap and bug fixes on the horizon)!
Now this readme can be used to track issues I'm aware of and lay out future steps for expansion! Plus, the "base layer" matter searches can now be used to filter by open matters specifically, which massively cuts down on execution time for GetAllRecordsAsync()
and FastFetch()!

## 1.0.0 - Publish to Live Environment!
Now the CalliAPICORE Project is live, so anyone in Amourgis & Associates can use it to compile reports and generate data! Tons of time saved and a whole bunch of redundant work made obsolete!

### 0.2.0 - Added Patch Log and Reporting! Plus, FastFetch!
This is exciting - now we have the FastFetch to grab all the records after a certain date AND a GetAllRecordsAsync() "Classic" GET.
FastFetch parallelizes the queries we send to Clio in order to grab up to 10,000 records lightning quick!

### v0.1.0 - Added Serilog for logging!

### v0 - Add MatterFilters to allow for easy, natural-language filtering!
