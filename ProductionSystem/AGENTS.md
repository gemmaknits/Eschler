# ProductionSystem Rules

- WinForms screens must use `Partial Class`, `.Designer.vb`, and `InitializeComponent()`.
- Keep controls/layout in `.Designer.vb`; keep runtime logic/data binding in the main `.vb` file.
- New or changed database access must use Stored Procedures through the class layer; do not add inline SQL to UI/Send flow.
- GMK Design Master uses `mas_dye_finishing_formula` through `p_finishing_group_select`.
- ESH currently uses the same Procedure name, but its Procedure returns `finishing_group`.
- Because the two masters/IDs differ, map ESH to GMK by `code`, never by numeric ID.
- `SendToGmkFlow` handles UI only; `classDesignNew` handles mapping and Stored Procedure calls.
- Before release, test Finishing codes `02` and `11`, run `git diff --check`, and build with the supported dependencies.
