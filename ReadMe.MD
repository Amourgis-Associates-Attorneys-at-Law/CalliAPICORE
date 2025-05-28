
# Wishlist

## Functionality
- Add error handling if the xlsx file being saved in ReportForm is already locked
- Add filters to TaskFilters

## UI
- Add a percentage to the progress bar (yeah it's a tiny thing but nice anyway)
- Make ONE Logger for the whole solution, and then pass it around to whatever needs it (dependency injection?)

## Documentation & External Resources

## Known Issues
- 3.0.3 - The Emergency Case Type is not yet translated into the MatterType picklist, so it will not show up properly in the UI. This is a known issue that will be addressed in a future update.

- 3.0.3 - The automatic update functionality is still in its early stages and may not work perfectly on all systems. If you encounter issues, please report them on the GitHub repository.

- 3.0.6 - The progress bar is doing something funky with the percentage

- 3.0.6 - The created_since field doesn't seem to work yet, probably needs normalizing into an ISO timestamp

# CalliAPI Patch Log

### 3.1 - CUSTOM REPORT BUILDER!!!!

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
