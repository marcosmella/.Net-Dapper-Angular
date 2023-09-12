import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";

import { LoadingSpinnerComponent } from "./../../../components/loading-spinner/loading-spinner.component";
import { ModalExamComponent } from "./modal-exam/modal-exam.component";
import { UploadFunctionService } from "./../../../services/upload-function.service";
import { ModalService } from "./../../../services/modal.service";
import { ElipsisAction, IconTypes } from "../../elipsis-grid/elipsis-grid.component";
import { MedicalExam } from "./../../../models/medical-exam.model";
import { MedicalExamService } from "./../../../services/medical-exam.service";
import { AlertService } from "./../../../services/alert.service";
import { SnackBarService } from "./../../../services/snack-bar.service";
import { UploadExamService } from "./../../../services/upload-exam.service";
import { FileType } from "./../../../models/file-type.model";

@Component({
	selector: "app-exams",
	templateUrl: "./exams.component.html",
	styleUrls: ["./exams.component.scss"]
})
export class ExamsComponent implements OnInit {
	@ViewChild("examSpinner", { static: true })
	examSpinner: LoadingSpinnerComponent;

	public globalActions: ElipsisAction[];
	public viewAsTable = false;
	public files: Array<any> = [];
	public viewAsTileDescription = "tile";
	public showGrid = false;
	public entityTypeIdEmployee = 1;
	public fileTypeId = null;
	public medicalExams: Array<MedicalExam> = [];
	public exams: Array<any> = [];
	public showElipsisOptions = false;
	public hasExams = false;
	public fileTypes: Array<FileType> = [];

	@Input() editing: boolean;
	@Input() idEmployee: number;
	constructor(
		public dialog: MatDialog,
		private uploadFunctionService: UploadFunctionService,
		private modalService: ModalService,
		private medicalExamService: MedicalExamService,
		private alertService: AlertService,
		private snackBarService: SnackBarService,
		private uploadExamService: UploadExamService
	) {}

	ngOnInit(): void {
		this.changeView(this.viewAsTileDescription);
		this.globalElipsis();
		this.getAllFiles();
		this.getFileTypes().then(() => {
			this.getExams().then(() => {
				this.getValueViewFiles();
			});
		});
	}

	getFileTypes(): Promise<any> {
		return new Promise((resolve, reject) => {
			this.uploadExamService.get().subscribe(
				(data) => {
					this.fileTypes = data;
					return resolve("ok");
				},
				(error) => {
					this.alertService.error(error.error.Error, "tableViewError");
					return reject();
				}
			);
		});
	}

	getExams(): Promise<any> {
		return new Promise((resolve, reject) => {
			this.medicalExamService.get().subscribe(
				(data) => {
					this.medicalExams = data.filter((item) => item.idEmployee === this.idEmployee);
					return resolve("ok");
				},
				(error) => {
					this.alertService.error(error.error.Error, "errorFiles");
					return reject();
				}
			);
		});
	}

	getValueViewFiles(): void {
		this.uploadFunctionService.getValueViewFiles().subscribe((data) => {
			if (data) {
				this.files = this.uploadFunctionService.files;
				this.setExamArray();
			}
		});
	}

	setExamArray(): Array<any> {
		this.exams = [];

		this.medicalExams.forEach((exam) => {
			const file = this.files.find((x) => x.id === exam.idFile);
			exam.type = this.fileTypes.find((item) => item.id === exam.idFileType).description;
			this.exams.push({
				exam: exam,
				file: file ? file : null
			});
		});
		this.hasExams = this.exams.length > 0;

		this.showElipsisOptions = this.exams.length > 0;

		return this.exams;
	}

	globalElipsis(): void {
		this.globalActions = new Array<ElipsisAction>();
		this.globalActions.push(
			{
				action: () => this.uploadFunctionService.downloadAll(),
				icon: IconTypes.download,
				description: "downloadAll"
			},
			{
				action: (element) => this.deleteAll(element),
				icon: IconTypes.delete,
				description: "deleteAll"
			}
		);
	}

	openModalExams(): void {
		const dialogRef = this.dialog.open(ModalExamComponent, {
			disableClose: true,
			hasBackdrop: true,
			width: "800px",
			height: "auto",
			data: { idEmployee: this.idEmployee, entityTypeIdEmployee: this.entityTypeIdEmployee, medicalExams: this.exams }
		});

		dialogRef.afterClosed().subscribe((resp) => {
			if (resp) {
				this.saveExamData(resp);
			}
		});
	}

	saveExamData(data: MedicalExam): Promise<any> {
		const method = data.id ? "put" : "post";
		return new Promise((resolve, reject) => {
			if (method === "put") {
				this.medicalExamService.put(data).subscribe(
					(response) => {
						this.snackBarService.openSnackBar({
							message: `modifiedSuccessfully`,
							icon: true,
							secondsDuration: 10,
							action: null
						});
						this.getAllFiles();
						this.getExams();
						return resolve(response);
					},
					() => {
						return reject();
					}
				);
			} else {
				this.medicalExamService.post(data).subscribe(
					(response) => {
						this.snackBarService.openSnackBar({
							message: `modifiedSuccessfully`,
							icon: true,
							secondsDuration: 10,
							action: null
						});
						this.getAllFiles();
						this.getExams();
						return resolve(response);
					},
					() => {
						return reject();
					}
				);
			}
		});
	}

	changeView(format: string): boolean {
		return (this.viewAsTable = format === "tile" ? true : false);
	}

	getAllFiles(): void {
		this.examSpinner.show();
		this.uploadFunctionService.getFiles(this.entityTypeIdEmployee, this.idEmployee, this.fileTypeId).finally(() => {
			this.examSpinner.hide();
		});
	}

	deleteAll(element: any): void {
		const promises: Array<Promise<any>> = [];
		this.modalService
			.openDialog({
				title: "atention",
				message: "areYouSureForDeleteAllfiles",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.examSpinner.show();
					element.forEach((index) => {
						if (index.file) {
							promises.push(
								this.uploadFunctionService.deleteAllFiles(this.entityTypeIdEmployee, this.idEmployee, index.file.id).then(() => {
									this.deleteExam(index.exam.id).then(() => {});
								})
							);
						} else {
							promises.push(this.deleteExam(index.exam.id).then(() => {}));
						}
					});
					this.examSpinner.hide();
					if (promises.length > 0) {
						Promise.all(promises)
							.then(() => {
								this.hasExams = false;
								this.showElipsisOptions = false;
								this.exams = [];
								this.files = [];
							})
							.catch((error) => {
								this.alertService.error(error.error.Error, "errorFiles");
							});
					}
				}
			});
	}

	deleteExam(id: number): Promise<any> {
		return new Promise((resolve, reject) => {
			this.medicalExamService.delete(id).subscribe(
				() => {
					return resolve("ok");
				},
				() => {
					return reject;
				}
			);
		});
	}
}
