import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { Subject } from "rxjs/internal/Subject";

import { LoadingSpinnerComponent } from "./../../../../components/loading-spinner/loading-spinner.component";
import { AlertService } from "./../../../../services/alert.service";
import { FileManagementService } from "./../../../../services/file-management.service";
import { UploadFileType } from "./../../../../models/upload-file-type.model";

@Component({
	selector: "app-exam-upload",
	templateUrl: "./exam-upload.component.html",
	styleUrls: ["./exam-upload.component.scss"]
})
export class ExamUploadComponent implements OnInit {
	@Input() showSpinner = true;

	@Output() onError: EventEmitter<string[]> = new EventEmitter<string[]>();

	@Output() idFile = new EventEmitter();

	@Input() hideButton: boolean;

	@Input() set imageDefault(url: string) {
		this.sourceFile = url;
	}

	@Input() set entityTypeId(entityTypeId: Number) {
		this.idEntityType = entityTypeId;
	}

	@Input() set fileTypeId(fileTypeId: Number) {
		this.idFileType = fileTypeId;
	}

	@Input() saved: boolean;

	@Input() set entityId(entityId: Number) {
		this.idEntity = entityId;
	}

	@Input() importFileName: string;

	@Input() id: string;

	@Input() styleInner: string;

	@Input() isUpdate: boolean;

	@Input() fileId: number;

	@ViewChild("spinnerExam", { static: true })
	public spinnerExam: LoadingSpinnerComponent;
	public sourceFile = "";
	public idEntity: Number;
	public idEntityType: Number;
	public idFileType: Number;
	public file: string;
	public isAllowed = false;
	public active = false;
	public acceptedFileTypes = [".pdf", ".jpeg", ".png", ".jpg"];
	public fileType = "file";
	public uploadFileType = UploadFileType;
	protected ngUnsubscribe$: Subject<void> = new Subject<void>();

	constructor(private alertService: AlertService, public fileManagementService: FileManagementService) {}

	ngOnInit(): void {}

	addFile(files: FileList): void {
		const fileToUpload = files.item(0);
		const reader = new FileReader();

		if (!this.isValidExtension(fileToUpload.name)) {
			this.alertService.warning(["importFileExtension"], "warningFileExtension");
			return;
		}

		reader.readAsDataURL(fileToUpload);
		reader.onload = (_event) => {
			this.sourceFile = <string>reader.result;
			if (this.isUpdate) {
				this.updateFile(this.fileId, fileToUpload);
			} else {
				this.sendFile(fileToUpload);
			}
		};
	}

	isValidExtension(fileName: string): boolean {
		const extension = fileName.substring(fileName.lastIndexOf(".")).toLowerCase();
		return this.acceptedFileTypes.indexOf(extension) > -1;
	}

	updateFile(fileId: number, fileToUpload: File): void {
		this.spinnerExam.show();
		this.fileManagementService
			.putFile(fileToUpload, this.idEntityType, this.idEntity, this.idFileType, this.fileType, this.active, fileId)
			.then((data) => {
				const resp = JSON.parse(data.response);
				if (resp.success) {
					this.saved = true;
					this.idFile.emit(resp.entity.fileId);
				} else if (!resp.values) {
					this.saved = false;
				}
			})
			.catch(() => {
				this.onError.emit(["uploadFileError"]);
				this.saved = false;
			})
			.finally(() => {
				this.spinnerExam.hide();
				this.isAllowed = true;
			});
	}

	sendFile(fileToUpload: File): void {
		this.spinnerExam.show();
		this.fileManagementService
			.postFile(fileToUpload, this.idEntityType, this.idEntity, this.idFileType, this.fileType, this.active)
			.then((data) => {
				const resp = JSON.parse(data.response);
				if (resp.success) {
					this.saved = true;
					this.idFile.emit(resp.entity.fileId);
				} else if (!resp.values) {
					this.saved = false;
				}
			})
			.catch(() => {
				this.onError.emit(["uploadFileError"]);
				this.saved = false;
			})
			.finally(() => {
				this.spinnerExam.hide();
				this.isAllowed = true;
			});
	}

	getFile(): void {
		this.sourceFile = "";
	}

	// tslint:disable-next-line: use-life-cycle-interface
	public ngOnDestroy(): void {
		this.ngUnsubscribe$.next();
		this.ngUnsubscribe$.complete();
	}
}
