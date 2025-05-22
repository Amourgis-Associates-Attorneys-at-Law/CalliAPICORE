Hello!

CalliAPI

# CalliAPI Patch Log

## 1.2.0 - Added UI implementation and KEYWORD SUPPORT!!!
I'm so pleased with this interface! The keyword inclusion is a gamechanger, and I'll continue moving reports into this TreeView UI so as to provide easy and clear definitions of terms!

## 1.1.0 - Wishlist (roadmap and bug fixes on the horizon)!
Now this readme can be used to track issues I'm aware of and lay out future steps for expansion! Plus, the "base layer" matter searches can now be used to filter by open matters specifically, which massively cuts down on execution time for GetAllRecordsAsync()
and FastFetch()!

## 1.0.0 - Publish to Live Environment!
Now the CalliAPICORE Project is live, so anyone in Amourgis & Associates can use it to compile reports and generate data! Tons of time saved and a whole bunch of redundant work made obsolete!

## 0.2.0 - Added Patch Log and Reporting! Plus, FastFetch!
This is exciting - now we have the FastFetch to grab all the records after a certain date AND a GetAllRecordsAsync() "Classic" GET.
FastFetch parallelizes the queries we send to Clio in order to grab up to 10,000 records lightning quick!

## v0.1.0 - Added Serilog for logging!

## v0 - Add MatterFilters to allow for easy, natural-language filtering!

# Wishlist

- Add error handling if the xlsx file being saved in ReportForm is already locked
- Add filters to TaskFilters
- Add a percentage to the progress bar (yeah it's a tiny thing but nice anyway)
- Make ONE Logger for the whole solution, and then pass it around to whatever needs it (dependency injection?)
- Publish the program to SharePoint as an easy to use executable?