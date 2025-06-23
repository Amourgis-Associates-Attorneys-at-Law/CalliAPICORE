<h2>Known Issues</h2>
There are no currently tracked issues. 
If you encounter unexpected behavior or issues, please report them to support@amourgis.com.



<h2>Roadmap</h2>
<strong>Functionality</strong>
📅4.7.0 - Allow users to access their calendars on the Custom Report Builder.
🖥️4.7.0 - Add the ability to create a Report using all of the data in a Matter lookup with the equivalent SQL information (as a checkbox 
in the Matters endpoint inside the CRB?).
- 4.7.3 - Add Responsible Attorney and Responsible Staff to the Matters fields.
- 4.7.3 - Add a check to the ReportForm to make sure the target .xlsx file isn't locked before saving.
- 5.0.5 - Change responsible staff and attorney by ID? It seems like it is possible to PATCH a matter.responsible_staff.id (long) value; 
if Clio doesn't have a way to do that natively, CalliAPI could theoretically do that. It's a long way off because CalliAPI is currently 
blind to users, but adding them by reference and then setting up some kind of routine to go through and return all matters where 
matter.responsible_staff.id == oldUserId and then for each matter patch matter.responsible_staff.id = newUserId could be a way to do it.

<strong>UI</strong>
📄4.7.0 - Include the parent field name when flattening to a ReportForm.
- 4.7.4 - Add a tutorial.

<h3>Documentation & External Resources</h3>
(none)