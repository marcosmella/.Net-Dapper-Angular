import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { NgxFileDropEntry, FileSystemFileEntry } from "ngx-file-drop";
import { SafeUrl } from "@angular/platform-browser";
import { MatDialog } from "@angular/material/dialog";

import { LoadingSpinnerComponent } from "./../../../components/loading-spinner/loading-spinner.component";
import { AlertService } from "./../../../services/alert.service";
import { DragDropFileService } from "./../../../services/drag-drop-file.service";
import { UploadFileParameter } from "./../../../models/upload-file-parameter.model";
import { ElipsisAction } from "./../../../components/elipsis-grid/elipsis-grid.component";
import { ModalFileViewerComponent } from "./../modal-file-viewer/modal-file-viewer.component";
import { TranslatePipe } from "./../../../pipes/translate.pipe";

@Component({
	selector: "app-drag-drop-file",
	templateUrl: "./drag-drop-file.component.html",
	styleUrls: ["./drag-drop-file.component.scss"]
})
export class DragDropFileComponent implements OnInit {
	@ViewChild("spinnerUploadFile", { static: true })
	public spinnerUploadFile: LoadingSpinnerComponent;
	public fileDrop: NgxFileDropEntry[] = [];
	public fileParametersToUpload = <UploadFileParameter>{};
	public saved = false;
	public showSaveIcon = false;
	public fileIcon: string;
	public fileExists = false;
	public fileId: number;
	public idEntityType: Number;
	public actions: ElipsisAction[];
	public data: string;
	public protectedUrl: SafeUrl;
	public fileDeleted = false;
	public dropFileType: string;
	public uploadFileFormatDescription = this.translatePipe.transform("uploadFileFormat");
	public fileDataName: string;
	public isUpdate = false;
	public isDelete = false;
	public fileData: any;
	public errorFile: boolean;
	@Input() acceptFormats: string[];
	@Input() entityTypeId: number;
	@Input() entityId: number;
	@Input() fileTypeId: number;
	@Input() fileType: string;
	@Input() active: boolean;
	@Input() set updateFileData(value: any) {
		if (value) {
			this.fileData = value;
			this.setEditFileProperties(value);
			this.fileId = value.exam ? value.exam.idFile : value.file.idFile;
		}
	}
	@Input() ellipsisActions = new Array<ElipsisAction>();
	@Input() id: string;
	@Input() multiple: boolean;
	@Output() fileName = new EventEmitter<string>();
	private isDropped = false;

	constructor(
		private dragDropFileService: DragDropFileService,
		private alertService: AlertService,
		public dialog: MatDialog,
		private translatePipe: TranslatePipe
	) {}

	ngOnInit(): void {
		this.setElipsis();
	}

	setElipsis(): void {
		this.actions = this.ellipsisActions;
	}

	public dropped(files: NgxFileDropEntry[]): void {
		this.isDelete = false;
		for (const droppedFile of files) {
			if (this.dragDropFileService.isFileAllowed(droppedFile.fileEntry.name, this.acceptFormats)) {
				if (droppedFile.fileEntry.isFile) {
					const fileEntry = <FileSystemFileEntry>droppedFile.fileEntry;
					fileEntry.file((file: File) => {
						const fileSize = this.dragDropFileService.allowedMaxFileSize(file.size);

						if (!fileSize) {
							this.errorFile = true;
							this.alertService.error(["uploadFileSize"], "uploadFileError");
						} else {
							this.isDropped = true;
							this.fileDeleted = false;
							this.fileParametersToUpload.fileToUpload = file;
							this.fileParametersToUpload.entityTypeId = this.entityTypeId;
							this.fileParametersToUpload.entityId = this.entityId;
							this.fileParametersToUpload.fileTypeId = this.fileTypeId;
							this.fileParametersToUpload.fileType = this.fileType;
							this.fileParametersToUpload.active = this.active;
							this.fileParametersToUpload.fileDescription = file.name;
							this.fileDrop = files;
							this.fileIcon = this.dragDropFileService.getFileIcon(this.fileDrop[0].relativePath);
							this.fileName.emit(file.name);
							this.dropFileType = this.dragDropFileService.checkFileType(file.name);
							this.errorFile = false;
						}
					});
				}
			} else {
				this.errorFile = true;
				this.alertService.error([`${this.uploadFileFormatDescription} ${this.acceptFormats}`], "uploadFileError");
			}
		}
	}

	save(): Promise<void> {
		this.spinnerUploadFile.show();

		return new Promise((resolve) => {
			const method = this.fileExists ? "updateParameters" : "postParameters";
			this[method]()
				.catch((error) => {
					this.alertService.error(error, "uploadFileError");
					this.saved = false;
					this.showSaveIcon = true;
				})
				.finally(() => {
					this.spinnerUploadFile.hide();
					return resolve(null);
				});
		});
	}

	updateParameters(): Promise<any> {
		return new Promise((resolve, reject) => {
			this.dragDropFileService
				.setUpdateParameters(this.fileId, this.fileParametersToUpload)
				.then((response) => {
					if (response.success) {
						this.setProperties(response);
						this.dragDropFileService.getfileById(response.entity.fileId, this.fileParametersToUpload).then((data) => {
							return resolve(data);
						});
					}
				})
				.catch((error) => {
					return reject(error);
				});
		});
	}

	postParameters(): Promise<any> {
		return new Promise((resolve, reject) => {
			this.dragDropFileService
				.setPostParameters(this.fileParametersToUpload)
				.then((response) => {
					this.setProperties(response);
					return resolve(null);
				})
				.catch((error) => {
					return reject(error);
				});
		});
	}

	openViewer(): void {
		const item = [];
		if (this.isUpdate && !this.isDropped) {
			item.push({
				dropFileType: this.dragDropFileService.checkFileType(this.fileData.file.urlName),
				file: this.fileData.file,
				isUpdate: true
			});
		} else {
			item.push({
				dropFileType: this.dropFileType,
				file: this.fileParametersToUpload.fileToUpload,
				isUpdate: false
			});
		}
		this.dialog.open(ModalFileViewerComponent, {
			disableClose: false,
			hasBackdrop: true,
			width: "800px",
			height: "auto",
			data: {
				item
			}
		});
	}

	download(): void {
		if (this.isUpdate) {
			this.dragDropFileService.download(this.fileData.file);
		} else {
			this.downloadFromMemory();
		}
	}

	downloadFromMemory(): void {
		const reader = new FileReader();
		reader.onload = () => {
			const url = reader.result;
			const type = url.toString().split(",")[0];
			const data = url.toString().split(",")[1];
			const dataURL = `${type},${data}`;
			const linkSource = `${dataURL}`;
			const downloadLink = document.createElement("a");
			const fileName = this.fileParametersToUpload.fileToUpload.name;
			downloadLink.href = linkSource;
			downloadLink.download = fileName;
			downloadLink.click();
		};
		reader.readAsDataURL(this.fileParametersToUpload.fileToUpload);
	}

	delete(): void {
		if (this.saved && !this.isDropped) {
			const acceptParams = this.fileData.file.file ? this.setDeleteParams(this.fileData.file.file) : this.fileData.file;

			this.fileId = this.fileData.file.id ? this.fileData.file.id : this.fileData.file.file.fileId;
			this.dragDropFileService.deleteFile(this.fileId, acceptParams).then((response) => {
				if (response.success) {
					this.showSaveIcon = false;
					this.fileDataName = "";
					this.fileIcon = "";
					this.isDelete = true;
					this.fileData = null;
					this.fileExists = false;
					this.fileName.emit("");
				}
			});
		} else {
			this.deleteFileInMemory();
		}
	}

	deleteFileInMemory(): void {
		this.fileDrop = [];
		this.fileIcon = "";
		this.data = "";
		this.protectedUrl = "";
		this.fileDeleted = true;
		this.fileName.emit("");
	}

	setDeleteParams(deleteParamsData: any): any {
		const acceptParams = {
			file: { entityId: deleteParamsData.entityId, type: "file" },
			entityTypeId: this.entityTypeId
		};
		return acceptParams;
	}

	setEditFileProperties(updateData: any): void {
		if (updateData) {
			this.saved = true;
			this.showSaveIcon = true;
			this.fileExists = true;
			this.fileIcon = updateData.file.iconClass.split("-")[1];
			this.fileDataName = `${updateData.file.file.description}.${this.fileIcon}`;
			this.isUpdate = true;
		}
	}

	setProperties(response: any): void {
		if (response) {
			this.saved = true;
			this.showSaveIcon = true;
			this.fileExists = true;
			this.fileId = response.entity.fileId;
		} else if (!response.values) {
			this.saved = false;
			this.showSaveIcon = true;
		}
	}
}
