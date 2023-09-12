import { MatDialog } from "@angular/material/dialog";
import { Component, ViewChild, Input, AfterViewInit } from "@angular/core";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";
import { Observable } from "rxjs/internal/Observable";

import { LoadingSpinnerComponent } from "../loading-spinner/loading-spinner.component";
import { NoteService } from "../../services/notes.service";
import { AlertService } from "../../services/alert.service";
import { NotesModalComponent } from "../notes/notes-modal/notes-modal.component";
import { Note } from "../../models/notes.model";
import { ModalService } from "../../services/modal.service";
import { SnackBarService } from "../../services/snack-bar.service";
import { TypesNoteColor } from "../../models/notes-color.model";
@Component({
	selector: "app-notes",
	templateUrl: "./notes.component.html",
	styleUrls: ["./notes.component.scss"]
})
export class NotesComponent implements AfterViewInit {
	@Input() set idEmployee(value: number) {
		if (value) {
			this._idEmployee = value;
			this.search(this._idEmployee);
		}
	}

	@Input() editing: boolean;

	public typeNotesColor: TypesNoteColor[];
	public notes: Array<Note> = [];
	@ViewChild("spinnerNotes", { static: false })
	public spinnerNotes: LoadingSpinnerComponent;
	public isConfidential: boolean;

	public spinner: Observable<Boolean>;
	private spinnerBehavior$ = new BehaviorSubject(false);
	private _idEmployee: number;

	constructor(
		private notesService: NoteService,
		private alertService: AlertService,
		private dialog: MatDialog,
		private modalService: ModalService,
		private snackBarService: SnackBarService
	) {
		this.typeNotesColor = [];
		this.spinner = this.spinnerBehavior$.asObservable();
	}

	ngAfterViewInit(): void {
		this.spinner.subscribe((val) => {
			if (val) {
				this.spinnerNotes.show();
			} else {
				this.spinnerNotes.hide();
			}
		});
	}

	createNote(): void {
		const dialogRef = this.dialog.open(NotesModalComponent, {
			disableClose: true,
			hasBackdrop: true,
			data: { isDisabled: false, idEmployee: this._idEmployee },
			width: "780px",
			height: "auto"
		});

		dialogRef.afterClosed().subscribe((data) => {
			if (data) {
				this.notes.push(data);
				this.search(this._idEmployee);
			}
		});
	}

	deleteNote(note: Note, index: number): void {
		this.modalService
			.openDialog({
				title: "atention",
				message: "areYouSureForDelete",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.spinnerNotes.show();
					this.notesService
						.delete(note.id, note.idPerson)
						.subscribe(
							() => {
								this.snackBarService.openSnackBar({
									message: `deletedSuccessfully`,
									icon: true,
									action: null,
									secondsDuration: 10
								});
								this.notes.splice(index, 1);
							},
							(error) => {
								this.alertService.error(error.error.Error);
							}
						)
						.add(() => {
							this.spinnerNotes.hide();
						});
				}
			});
	}

	editNote(index: number): void {
		const dialogRef = this.dialog.open(NotesModalComponent, {
			disableClose: true,
			hasBackdrop: true,
			data: { note: this.notes[index], isDisabled: false, idEmployee: this._idEmployee },
			width: "780px",
			height: "auto"
		});

		dialogRef.afterClosed().subscribe((data) => {
			if (data) {
				this.notes[index] = data;
				this.search(this._idEmployee);
			}
		});
	}

	viewNote(index: number): void {
		if (this.editing) {
			this.dialog.open(NotesModalComponent, {
				data: { note: this.notes[index], isDisabled: true },
				width: "780px",
				height: "auto"
			});
		}
	}

	markAsReview(index: number): void {
		const objectModified = this.notes[index];
		objectModified.revision = !objectModified.revision;
		objectModified["idNoteType"] = objectModified.type.id;
		this.notesService.put(objectModified).subscribe(
			() => {
				this.snackBarService.openSnackBar({
					message: `modifiedSuccessfully`,
					icon: true,
					action: null,
					secondsDuration: 10
				});
			},
			(error) => {
				this.alertService.error(error.error.Error);
			}
		);
	}

	public getRandomColor(idType: number): string {
		let color = "#";
		let colorValue = this.typeNotesColor.find((x) => x.id === idType);
		if (!colorValue) {
			const letters = "0123456789ABCDEF".split("");
			for (let i = 0; i < 6; i++) {
				color += letters[Math.floor(Math.random() * 16)];
			}
			this.typeNotesColor.push({ id: idType, color: color });
			colorValue = this.typeNotesColor.find((x) => x.id === idType);
		}
		return colorValue.color;
	}

	search(idEmployee: number): void {
		this.spinnerBehavior$.next(true);
		this.notesService
			.get(idEmployee)
			.subscribe(
				(data: Note[]) => {
					this.notes = data;
					this.notes.forEach((element) => {
						element.color = this.getRandomColor(element.type.id);
					});
				},
				(error) => {
					if (error.status === 404) {
						this.notes = [];
						return;
					}
					this.alertService.error(error.error.Error);
				}
			)
			.add(() => {
				this.spinnerBehavior$.next(false);
			});
	}
}
