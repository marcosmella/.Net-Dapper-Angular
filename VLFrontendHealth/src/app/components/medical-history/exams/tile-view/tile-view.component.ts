import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";

import { ElipsisAction, IconTypes } from "./../../../../components/elipsis-grid/elipsis-grid.component";
import { LoadingSpinnerComponent } from "./../../../../components/loading-spinner/loading-spinner.component";
import { EnumFileType } from "./../../../../models/file-type.model";
import { ModalService } from "./../../../../services/modal.service";
import { UploadFileType } from "./../../../../models/upload-file-type.model";
import { UploadFunctionService } from "./../../../../services/upload-function.service";
import { MedicalExamService } from "./../../../../services/medical-exam.service";
import { ModalExamComponent } from "../modal-exam/modal-exam.component";
import { MedicalExam } from "./../../../../models/medical-exam.model";
import { AlertService } from "./../../../../services/alert.service";

@Component({
	selector: "app-tile-view",
	templateUrl: "./tile-view.component.html",
	styleUrls: ["./tile-view.component.scss"]
})
export class TileViewComponent implements OnInit {
	@Input() editing: boolean;

	@Input("files") set Files(value: Array<any>) {
		this.files = value;
	}

	@Input("exams") set Exams(value: Array<any>) {
		this.exams = value;
	}

	@Input("idEmployee") set IdEmployee(value: number) {
		this.idEmployee = value;
	}
	@Output() modalExam = new EventEmitter<MedicalExam>();
	@Output() updateTable = new EventEmitter();

	@ViewChild("tileSpinner", { static: true })
	tileSpinner: LoadingSpinnerComponent;

	public idEmployee: number;
	public tileActions: ElipsisAction[];
	public files: Array<any> = [];
	public exams: Array<any> = [];
	public fileTypeExtension: string;
	public employeeEntityTypeId = 1;
	public fileTypeId = null;
	public fileType = EnumFileType;
	public uploadFileType = UploadFileType;

	constructor(
		public dialog: MatDialog,
		private modalService: ModalService,
		private uploadFunctionService: UploadFunctionService,
		private medicalExamService: MedicalExamService,
		private alertService: AlertService
	) {}

	ngOnInit(): void {
		this.fileElipsis();
		this.uploadFunctionService.getValueViewFiles().subscribe((data) => {
			if (data) {
				this.files = this.uploadFunctionService.files;
			}
		});
	}

	getAllFiles(): void {
		this.tileSpinner.show();
		this.uploadFunctionService.getFiles(this.employeeEntityTypeId, this.idEmployee, this.fileTypeId).finally(() => {
			this.tileSpinner.hide();
		});
	}

	fileElipsis(): void {
		this.tileActions = new Array<ElipsisAction>();
		this.tileActions.push(
			{
				action: (element) => this.uploadFunctionService.view(element.file.id),
				icon: IconTypes.view
			},
			{
				action: (element) => this.modifyExam(element.exam.id),
				icon: IconTypes.edit
			},
			{
				action: (element) => this.uploadFunctionService.download(element.file.id),
				icon: IconTypes.download
			},
			{
				action: (element) => this.delete(element),
				icon: IconTypes.delete
			}
		);
	}

	delete(element: any): void {
		this.modalService
			.openDialog({
				title: "atention",
				message: "areYouSureForDelete",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.deleteFile(element, this.employeeEntityTypeId, this.idEmployee);
				}
			});
	}

	deleteFile(element: any, idEntityType: number, idEmployee: number): void {
		this.tileSpinner.show();
		if (!element.file) {
			this.deleteExam(element.exam.id);
		} else {
			this.uploadFunctionService
				.delete(element.file, idEntityType, idEmployee)
				.then(() => {
					this.deleteExam(element.exam.id);
				})
				.catch((err) => {
					this.alertService.error(err.error.Error, "errorFiles");
				})
				.finally(() => {
					this.tileSpinner.hide();
				});
		}
	}

	deleteExam(idExam: number): void {
		this.medicalExamService
			.delete(idExam)
			.subscribe(
				() => {
					this.getAllFiles();

					this.updateTable.emit();
				},

				(err) => {
					this.alertService.error(err.error.Error, "errorFiles");
				}
			)
			.add(() => {
				this.tileSpinner.hide();
			});
	}

	modifyExam(index: number): void {
		const exam = this.exams.find((x) => x.exam.id === index);
		const dialogRef = this.dialog.open(ModalExamComponent, {
			disableClose: true,
			hasBackdrop: true,
			data: {
				exam: exam,
				idEmployee: this.idEmployee,
				entityTypeIdEmployee: this.employeeEntityTypeId,
				fileId: exam.file?.id,
				medicalExams: this.exams
			},
			width: "780px",
			height: "auto"
		});

		dialogRef.afterClosed().subscribe((data: MedicalExam) => {
			if (data) {
				this.modalExam.emit(data);
			}
		});
	}
}
