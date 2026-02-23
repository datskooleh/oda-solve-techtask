# oda-solve-techtask# oda-solve-techtask

This is implementation for the technical task.

Initial information:
1. Main object is hospital.
2. Hospital can have multiple departments.
3. Patient is assigned to a specific department.

Requirements (general):
1. Department can be added/updated/removed (CRUD).
2. Partient can be moved added/updated/removed.
3. Patient can be re-assigned to another department
4. Filter should be in place to return patiends by department and give date.

Requirement for department:
1. No departments with 2 same name can exist

Requirement for patient
1. Removal should not remove user from database but should "discharge" him from hospital.
2. Audit of every change related to patient should be perserved.
3. Optional description should be provided during department re-assignment 


Acceptance criteria:
1. Department can be added, updated and removed (discharched).
2. Department can't be removed if there are patients assigned.
3. Patient can be added, updated and removed.
4. Log trace of patient change (add, update, discharge) should be perserved in the separate table.
5. Patient can be searcheable by department and name.
6. Patient can be reassigned to another department.

Data Structure  
Patient  
- AdmissionNumber (string, e.g. "88783/1")
- PatientName (string, e.g. "Mustermann, Max")

Department  
- ShortName (string, e.g. "CHIR")
- LongName (string, e.g. "Allgemeine Chirurgie")
