import { Component, Inject, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import * as moment from "moment";
import { MedicalExam } from "src/app/models/medical-exam.model";

import { DateValidators } from "./../../../../validators/date-validator";
import { AlertService } from "./../../../../services/alert.service";
import { FileType } from "./../../../../models/file-type.model";
import { UploadExamService } from "./../../../../services/upload-exam.service";
import { ModalComponent } from "./../../../../components/modal/modal.component";
import { LoadingSpinnerComponent } from "./../../../../components/loading-spinner/loading-spinner.component";
import { ElipsisAction, IconTypes } from "./../../../../components/elipsis-grid/elipsis-grid.component";
import { DragDropFileComponent } from "./../../../../shared/components/drag-drop-file/drag-drop-file.component";

@Component({
	selector: "app-modal-exam",
	templateUrl: "./modal-exam.component.html",
	styleUrls: ["./modal-exam.component.scss"]
})
export class ModalExamComponent implements OnInit {
	@ViewChild("spinner", { static: false })
	public spinner: LoadingSpinnerComponent;

	public formExamUpload: FormGroup;
	public entityTypeIdEmployee: number;
	public dateNow = new Date().At0Time();
	public fileTypeDescription: Array<FileType> = [];
	public idFileTypeExam = 10001;
	public idEmployee: number;
	public fileId: number;
	public formData: any;
	public isUpdate = false;
	public acceptFormats: string[] = [".png", ".jpg", ".pdf", ".jpeg"];
	public active = false;
	public fileType = "file";
	@ViewChild("dragAndDropFile", { static: false })
	public dragAndDropFile: DragDropFileComponent;
	public ellipsisActions = new Array<ElipsisAction>();
	public updateData: any;
	public showDragArea = true;
	public isDisabledExpirationDate = true;
	public duplicatedExams: boolean;

	constructor(
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialogRef: MatDialogRef<ModalComponent>,
		public alertService: AlertService,
		private fb: FormBuilder,
		private dateValidators: DateValidators,
		private uploadExamService: UploadExamService
	) {
		this.idEmployee = this.data.idEmployee;
		this.entityTypeIdEmployee = this.data.entityTypeIdEmployee;
		this.fileId = this.data.fileId;
		this.setForm();
	}

	ngOnInit(): void {
		this.getFileType();
		if (this.data.exam) {
			this.isUpdate = true;
			this.formExamUpload = this.fb.group({
				idFileType: [this.data.exam.exam.idFileType, [Validators.required]],
				examDate: [this.data.exam.exam.examDate, [Validators.required]],
				expirationDate: [this.data.exam.exam.expirationDate, [this.dateValidators.dateLessEqualThan("examDate", "expirationDate")]],
				idFile: this.data.exam.exam.idFile,
				idEmployee: this.data.exam.exam.idEmployee,
				id: this.data.exam.exam.id
			});
			if (this.data.exam.file) {
				this.updateData = this.data.exam;
			}
		}

		this.setEllipsis();
	}

	setForm(): void {
		this.formExamUpload = this.fb.group({
			idFileType: [null, [Validators.required]],
			examDate: [null, Validators.required],
			expirationDate: [null, [this.dateValidators.dateLessEqualThan("examDate", "expirationDate")]],
			filename: [null]
		});
	}

	cancel(): void {
		this.dialogRef.close();
	}

	getFileType(): void {
		this.uploadExamService.get().subscribe(
			(data: FileType[]) => {
				this.fileTypeDescription = data.filter((x) => x.id === this.idFileTypeExam);
			},
			(error) => {
				this.alertService.error(error.error.Error, "examError");
			}
		);
	}

	errorImage(message: string[]): void {
		this.alertService.error(message, "examError");
	}

	submit(): void {
		if (this.formExamUpload.valid) {
			const idExam = this.isUpdate ? this.data.exam.exam.id : 0;

			const examDate = this.formExamUpload.get("examDate").value;
			const idFileType = this.formExamUpload.get("idFileType").value;

			if (this.examAlreadyExist(idFileType, examDate, idExam) === false) {
				if (this.dragAndDropFile.fileParametersToUpload.fileDescription) {
					this.dragAndDropFile.save().then(() => {
						this.setExamParameters();
					});
				} else {
					this.setExamParameters();
				}
			} else {
				this.duplicatedExams = true;
				this.formExamUpload.markAllAsTouched();
				this.alertService.error(["examAlreadyExist"], "examError");
			}
		} else {
			this.formExamUpload.markAllAsTouched();
			this.alertService.clear();
		}
	}

	setExamParameters(): void {
		let medicalExam = <MedicalExam>{};
		if (this.isUpdate) {
			medicalExam = this.formExamUpload.value;
			if (this.dragAndDropFile.fileId) {
				if (this.dragAndDropFile.isDelete) {
					medicalExam.idFile = 0;
				} else {
					medicalExam.idFile = this.dragAndDropFile.fileId;
				}
			} else {
				medicalExam.idFile = this.data.exam.exam.idFile ? this.data.exam.exam.idFile : 0;
			}
			medicalExam.idEmployee = this.data.exam.exam.idEmployee;
			medicalExam.id = this.data.exam.exam.id;
		} else {
			medicalExam.idEmployee = this.data.idEmployee;
			medicalExam.idFileType = this.formExamUpload.get("idFileType").value;
			if (this.dragAndDropFile.fileId) {
				medicalExam.idFile = this.dragAndDropFile.fileId;
			} else {
				medicalExam.idFile = 0;
			}
			medicalExam.expirationDate = this.formExamUpload.get("expirationDate").value;
			medicalExam.examDate = this.formExamUpload.get("examDate").value;
		}

		this.dialogRef.close(medicalExam);
	}

	examAlreadyExist(idFileType: number, examDate: Date, idExam?: number): boolean {
		const paramExamDate = moment(examDate).format("YYYY/MM/DD");
		let existExam = false;

		for (const exam of this.data.medicalExams) {
			const fecha = moment(exam.exam.examDate).format("YYYY/MM/DD");
			if (exam.exam.idFileType === idFileType && paramExamDate === fecha && exam.exam.id !== idExam) {
				existExam = true;
				break;
			}
		}

		return existExam;
	}

	setEllipsis(): void {
		this.ellipsisActions.push(
			{
				action: () => {
					this.dragAndDropFile.openViewer();
				},
				icon: IconTypes.view,
				description: "view"
			},
			{
				action: () => {
					this.dragAndDropFile.download();
				},
				icon: IconTypes.download,
				description: "download"
			},
			{
				action: () => {
					this.dragAndDropFile.delete();
				},
				icon: IconTypes.delete,
				description: "delete"
			}
		);
	}

	setFilename(event?: Event): void {
		if (this.isUpdate && this.formExamUpload.valid) {
			Object.keys(this.formExamUpload.controls).forEach((key) => {
				this.formExamUpload.controls[key].markAsDirty();
			});
		}

		this.formExamUpload.patchValue({
			filename: event ? event : null
		});
	}

	showDragAndDrop(event: any): void {
		if (event.id) {
			this.showDragArea = false;
		}
	}

	enableExpirationDate(): void {
		this.setVariableDuplicatedExams();
		const examDateControl = this.formExamUpload.controls["examDate"];
		const expirationDateControl = this.formExamUpload.controls["expirationDate"];
		if (examDateControl.value === null) {
			expirationDateControl.patchValue("");
			this.isDisabledExpirationDate = true;
		} else {
			this.isDisabledExpirationDate = false;
		}
		this.checkValidators();
	}

	setVariableDuplicatedExams(): void {
		if (this.formExamUpload.valid && this.duplicatedExams) {
			this.duplicatedExams = false;
		}
	}

	checkValidators(): void {
		this.formExamUpload.get("expirationDate").valueChanges.subscribe((data) => {
			if (!data) {
				this.formExamUpload.controls["expirationDate"].patchValue("", { emitEvent: false });
			}
		});

		Object.keys(this.formExamUpload.controls).forEach((field) => {
			const control = this.formExamUpload.get(field);
			control.updateValueAndValidity();
		});
	}
}
