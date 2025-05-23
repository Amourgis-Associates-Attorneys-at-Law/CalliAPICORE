Hello!

# Notes to self
Publish using 
msbuild CalliAPI.csproj /t:Publish /p:PublishProfile=GitHubPages /p:Configuration=Release /p:NoWarn=true /v:normal > publish.log

CalliAPI

# CalliAPI Patch Log

## 1.8.0 - Prepping framework for a new way to install and landing pages!
Now CalliAPI has a more permanent home inside GitHub Pages with a custom-made index page for installing from!
(Don't ask about 1.5 and 1.6 and 1.7, it was all suffering with MSBuild, but now I know more I suppose!)

## 1.4.0 - Glow up in preparation for version 2!
A new UI, a new icon, and a new way of installing gives CalliAPI a lot more staying power! Couple that with the new Publish.html page and we have a way more user-friendly experience!

## 1.3.0 - Added VersionHelper and fixed some bugs!
Added a VersionHelper to help with maintaining version control. The ClickOnce version should now be the same as the version in the .csproj file, so we should be pretty good to go with updates and version control moving forward!

### 1.2.1 - Centralized version numbering, expanded filters!
Added display_name to the fields we can parse from Clio into Matters, so now we have our own ideal way of sorting and identifying clients! It also gives us the ability to now FILTER by display_name, which is a huge win!

## 1.2.0 - Added UI implementation and KEYWORD SUPPORT!!!
I'm so pleased with this interface! The keyword inclusion is a gamechanger, and I'll continue moving reports into this TreeView UI so as to provide easy and clear definitions of terms!

## 1.1.0 - Wishlist (roadmap and bug fixes on the horizon)!
Now this readme can be used to track issues I'm aware of and lay out future steps for expansion! Plus, the "base layer" matter searches can now be used to filter by open matters specifically, which massively cuts down on execution time for GetAllRecordsAsync()
and FastFetch()!

## 1.0.0 - Publish to Live Environment!
Now the CalliAPICORE Project is live, so anyone in Amourgis & Associates can use it to compile reports and generate data! Tons of time saved and a whole bunch of redundant work made obsolete!

### 0.2.0 - Added Patch Log and Reporting! Plus, FastFetch!
This is exciting - now we have the FastFetch to grab all the records after a certain date AND a GetAllRecordsAsync() "Classic" GET.
FastFetch parallelizes the queries we send to Clio in order to grab up to 10,000 records lightning quick!

### v0.1.0 - Added Serilog for logging!

### v0 - Add MatterFilters to allow for easy, natural-language filtering!

# Wishlist

## Functionality
- Add error handling if the xlsx file being saved in ReportForm is already locked
- Add filters to TaskFilters

## UI
- Add a percentage to the progress bar (yeah it's a tiny thing but nice anyway)
- Make ONE Logger for the whole solution, and then pass it around to whatever needs it (dependency injection?)

## Documentation & External Resources
- Add a zip to the docs folder so that users can fallback on the zip
