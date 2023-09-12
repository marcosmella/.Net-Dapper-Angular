import { Component, OnInit, ViewChild, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";

import { NoteService } from "../../../services/notes.service";
import { NoteType } from "../../../models/note-type.model";
import { LoadingSpinnerComponent } from "./../../loading-spinner/loading-spinner.component";
import { ModalComponent } from "./../../modal/modal.component";
import { AlertService } from "./../../../services/alert.service";
import { SnackBarService } from "./../../../services/snack-bar.service";
import { NoteTypeService } from "./../../../services/note-type.service";

@Component({
	selector: "app-notes-modal",
	templateUrl: "./notes-modal.component.html",
	styleUrls: ["./notes-modal.component.scss"]
})
export class NotesModalComponent implements OnInit {
	public noteTypes: Array<NoteType> = [];
	public formNotes: FormGroup;

	@ViewChild("spinner", { static: false })
	public spinner: LoadingSpinnerComponent;
	public isUpdate = false;
	public formDisabled = false;

	constructor(
		private notesService: NoteService,
		private snackBarService: SnackBarService,
		private fb: FormBuilder,
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialogRef: MatDialogRef<ModalComponent>,
		private alertService: AlertService,
		private noteTypeService: NoteTypeService
	) {
		this.formNotes = this.fb.group({
			id: [0],
			idPerson: [0],
			idNoteType: [null, [Validators.required]],
			creation: [null, [Validators.required]],
			expiration: [null],
			motive: [null, [Validators.required, Validators.maxLength(50)]],
			description: [null, [Validators.required, Validators.maxLength(1000)]],
			revision: [false],
			color: [""]
		});
	}

	enabledForm(): void {
		if (this.data.isDisabled) {
			this.formDisabled = true;
			this.formNotes.disable();
		} else {
			this.formDisabled = false;
			this.formNotes.enable();
		}
	}

	getTypeNote(): void {
		this.noteTypeService.get().subscribe(
			(data) => {
				this.noteTypes = data;
			},
			(error) => {
				this.alertService.error(error.error.Error, "errorNotes");
			}
		);
	}

	ngOnInit(): void {
		this.enabledForm();

		this.getTypeNote();

		if (this.data.note) {
			this.isUpdate = true;
			this.formNotes.reset({ ...this.data.note, idNoteType: this.data.note.type.id });
		}

		this.formNotes.get("idPerson").setValue(this.data.idEmployee);
	}

	close(): void {
		this.dialogRef.close();
	}

	submit(): void {
		if (!this.formNotes.valid) {
			this.formNotes.markAllAsTouched();
			return;
		}

		this.spinner.show();
		if (this.isUpdate) {
			this.notesService
				.put(this.formNotes.value)
				.subscribe(
					() => {
						this.snackBarService.openSnackBar({
							message: `modifiedSuccessfully`,
							icon: true,
							secondsDuration: 10,
							action: null
						});
						this.dialogRef.close({ ...this.formNotes.value, type: { id: this.formNotes.value.idNoteType } });
					},
					(error) => {
						this.alertService.error(error.error.Error, "errorNotes");
					}
				)
				.add(() => {
					this.spinner.hide();
				});
		} else {
			this.notesService
				.post(this.formNotes.value)
				.subscribe(
					(id) => {
						this.snackBarService.openSnackBar({
							message: `createdSuccessfully`,
							icon: true,
							secondsDuration: 10,
							action: null
						});
						this.dialogRef.close({ ...this.formNotes.value, id: id, type: { id: this.formNotes.value.idNoteType } });
					},
					(error) => {
						this.alertService.error(error.error.Error, "errorNotes");
					}
				)
				.add(() => {
					this.spinner.hide();
				});
		}
	}
}
