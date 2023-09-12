import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";

import { ElipsisAction, IconTypes } from "./../../../../components/elipsis-grid/elipsis-grid.component";
import { LoadingSpinnerComponent } from "./../../../../components/loading-spinner/loading-spinner.component";
import { EnumFileType, FileType } from "./../../../../models/file-type.model";
import { UploadFileType } from "./../../../../models/upload-file-type.model";
import { ModalService } from "./../../../../services/modal.service";
import { UploadFunctionService } from "./../../../../services/upload-function.service";
import { ModalExamComponent } from "../modal-exam/modal-exam.component";
import { MedicalExam } from "./../../../../models/medical-exam.model";
import { MedicalExamService } from "./../../../../services/medical-exam.service";
import { AlertService } from "./../../../../services/alert.service";
import { UserPreference } from "./../../../../models/user-preference.model";

@Component({
	selector: "app-table-view",
	templateUrl: "./table-view.component.html",
	styleUrls: ["./table-view.component.scss"]
})
export class TableViewComponent implements OnInit {
	@Input() editing: boolean;

	@Input("files") set Files(value: Array<any>) {
		this.files = value;
	}

	@Input("exams") set Exams(value: Array<any>) {
		this.exams = value;
		this.getExams();
	}

	@Input("idEmployee") set IdEmployee(value: number) {
		this.idEmployee = value;
	}
	@Output() modalExam = new EventEmitter<MedicalExam>();
	@Output() updateTable = new EventEmitter();

	@ViewChild("tableSpinner", { static: true })
	tableSpinner: LoadingSpinnerComponent;

	@ViewChild(MatSort, { static: false })
	public sort: MatSort;

	@ViewChild(MatPaginator, { static: true })
	public paginator: MatPaginator;

	public idEmployee: number;
	public tableViewActions: ElipsisAction[];
	public files: Array<any> = [];
	public exams: Array<any> = [];

	public tableViewDisplayedColumns: string[] = ["file", "examType", "name", "examDate", "expirationDate", "options"];
	public tableViewDataSource: MatTableDataSource<any>;
	public recordsCount: number;
	public fileTypeExtension: string;
	public fileType = EnumFileType;
	public url: string;
	public employeeEntityTypeId = 1;
	public fileTypeId = null;
	public uploadFileType = UploadFileType;
	public fileTypes: Array<FileType> = [];

	constructor(
		public dialog: MatDialog,
		public userPreference: UserPreference,
		private modalService: ModalService,
		private uploadFunctionService: UploadFunctionService,
		private medicalExamService: MedicalExamService,
		private alertService: AlertService
	) {
		this.tableViewDataSource = new MatTableDataSource<any>();
	}

	ngOnInit(): void {
		this.fileElipsis();

		this.uploadFunctionService.getValueViewFiles().subscribe((data) => {
			if (data) {
				this.files = this.uploadFunctionService.files;
			}
		});
	}

	getExams(): void {
		this.tableViewDataSource.data = this.exams.map((element) => {
			element.image = element.file?.file.url;
			element.name = element.file?.file?.description;
			element.type = element.exam.type;
			element.examDate = element.exam.examDate;
			element.expirationDate = element.exam.expirationDate;
			return element;
		});

		this.tableViewDataSource.paginator = this.paginator;
		this.tableViewDataSource.sort = this.sort;
		this.recordsCount = this.tableViewDataSource.data.length;
	}

	getAllFiles(): void {
		this.tableSpinner.show();

		this.uploadFunctionService.getFiles(this.employeeEntityTypeId, this.idEmployee, this.fileTypeId).finally(() => {
			this.tableSpinner.hide();
		});
	}

	fileElipsis(): void {
		this.tableViewActions = new Array<ElipsisAction>();
		this.tableViewActions.push(
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
		this.tableSpinner.show();
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
					this.tableSpinner.hide();
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
				this.tableSpinner.hide();
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
