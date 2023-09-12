import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs/internal/BehaviorSubject";
import { Observable } from "rxjs/internal/Observable";
import * as JSZip from "jszip";
import * as FileSaver from "file-saver";
import { MatDialog } from "@angular/material/dialog";

import { FileManagementService } from "./../services/file-management.service";
import { SnackBarService } from "./../services/snack-bar.service";
import { UploadFileType } from "./../models/upload-file-type.model";
import { AlertService } from "./../services/alert.service";
import { EnumFileType, FileIconType } from "./../models/file-type.model";
import { AuthService } from "./../services/auth.service";
import { ImageView } from "./../models/image-view.model";
import { DragDropFileService } from "./drag-drop-file.service";
import { ModalFileViewerComponent } from "../shared/components/modal-file-viewer/modal-file-viewer.component";

@Injectable({
	providedIn: "root"
})
export class UploadFunctionService {
	public employeeEntityTypeId = 1;
	public uploadFileType = UploadFileType;
	public fileType = EnumFileType;
	public fileTypeExtension: string;
	public iconType = FileIconType;
	public files = [];
	private viewFilesSubjectChange$: BehaviorSubject<boolean>;
	private url: string;

	constructor(
		private snackBarService: SnackBarService,
		private fileManagementService: FileManagementService,
		private alertService: AlertService,
		private authServ: AuthService,
		private dragDropFileService: DragDropFileService,
		public dialog: MatDialog
	) {
		this.viewFilesSubjectChange$ = new BehaviorSubject<boolean>(false);
	}

	delete(files: any, employeeEntityTypeId: number, idEmployee: number): Promise<any> {
		return new Promise((resolve, reject) => {
			this.fileManagementService
				.deleteFile(employeeEntityTypeId, idEmployee, files.id, this.uploadFileType.file)
				.then(() => {
					this.snackBarService.openSnackBar({
						message: "deletedSuccessfully",
						icon: true,
						action: null,
						secondsDuration: 10
					});
					return resolve("ok");
				})
				.catch((error) => {
					this.alertService.error(error.error.Error, "errorFiles");
					return reject();
				});
		});
	}

	getFiles(employeeEntityTypeId: number, idEmployee: number, fileTypeId: number): Promise<any> {
		const active = false;
		fileTypeId = 10001;
		return new Promise((resolve, reject) => {
			this.fileManagementService.getfile(employeeEntityTypeId, idEmployee, fileTypeId, active, this.uploadFileType.file).subscribe(
				(data) => {
					this.files = [];
					if (data.totalCount > 0) {
						data.values.forEach((element) => {
							if (element.fileTypeId === fileTypeId) {
								const extension = element.url.split(".").pop();
								if (this.fileType[extension] === this.uploadFileType.image) {
									this.fileTypeExtension = this.uploadFileType.image;
								} else {
									this.fileTypeExtension = this.uploadFileType.file;
								}
								const iconClass = this.iconType[extension];
								const imageView: ImageView = {
									idEntity: idEmployee,
									entityTypeId: employeeEntityTypeId,
									fileTypeId: fileTypeId,
									active: active,
									type: this.uploadFileType.file,
									fileId: element.fileId
								};
								this.files.push({
									file: element,
									dataImage: imageView,
									urlName: this.getFileName(element.url),
									iconClass: iconClass,
									id: element.fileId,
									entityTypeId: this.employeeEntityTypeId,
									fileTypeExtension: this.fileTypeExtension,
									url: element.url
								});
							}
						});
					}
					this.setValueListenerViewFiles();
					return resolve("ok");
				},
				(error) => {
					this.alertService.error(error.error.Error, "errorFiles");
					return reject();
				}
			);
		});
	}

	getFileName(url: string): string {
		const urlSplit = url.split("/");
		return urlSplit[urlSplit.length - 1];
	}

	getValueViewFiles(): Observable<boolean> {
		return this.viewFilesSubjectChange$.asObservable();
	}

	setValueListenerViewFiles(): void {
		return this.viewFilesSubjectChange$.next(true);
	}

	download(fileId: number): void {
		const file = this.files.find((x) => x.id === fileId);
		const extension = file.urlName.split(".").pop();
		if (this.fileType[extension] === "image") {
			this.downloadImage(fileId);
		} else {
			this.downloadDocument(fileId);
		}
	}

	downloadImage(fileId: number): void {
		const file = this.files.find((x) => x.file.fileId === fileId);
		this.url = file.file.url;
		const extension = file.urlName.split(".").pop();
		const image = this.getDataImage(this.url, this.authServ.loadToken(), file.file.description, extension);
		window.open(image, "_self");
	}

	downloadDocument(fileId: number): void {
		const file = this.files.find((x) => x.file.fileId === fileId);
		this.url = file.file.url;
		this.getDataFile(file.file);
	}

	getDataFile(file: any): void {
		const xmlHTTP = new XMLHttpRequest();
		xmlHTTP.open("GET", file.url, true);
		xmlHTTP.responseType = "blob";
		xmlHTTP.onreadystatechange = () => {
			const promiseReady = 4;
			const statusOk = 200;
			if (xmlHTTP.readyState === promiseReady && xmlHTTP.status === statusOk) {
				const downloadUrl = URL.createObjectURL(xmlHTTP.response);
				const extension = file.url.split(".").pop();
				const a = document.createElement("a");
				document.body.appendChild(a);
				a.href = downloadUrl;
				a.download = `${file.description}-${file.fileTypeDescription}.${extension}`;
				a.click();
			}
		};

		xmlHTTP.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);
		xmlHTTP.send();
	}

	getDataImage(url: string, token: any, description: string, extension: string): any {
		if (this.doesFileExist(url, token)) {
			const xmlHTTP = new XMLHttpRequest();
			let dataURL: any;
			xmlHTTP.open("GET", url, true);
			xmlHTTP.responseType = "arraybuffer";
			xmlHTTP.onreadystatechange = () => {
				const promiseReady = 4;
				const statusOk = 200;
				if (xmlHTTP.readyState === promiseReady && xmlHTTP.status === statusOk) {
					const b64 = btoa(
						new Uint8Array(xmlHTTP.response).reduce(function (data: any, byte: any): any {
							return data + String.fromCharCode(byte);
						}, "")
					);

					extension = extension === "png" ? extension : "jpeg";
					dataURL = `data:image/${extension};base64,${b64}`;
					const enlace = document.createElement("a");
					enlace.href = dataURL;
					enlace.download = description ? description : this.getFileName(url);
					document.body.appendChild(enlace);
					enlace.click();
					enlace.parentNode.removeChild(enlace);
				}
			};
			xmlHTTP.setRequestHeader("Authorization", `Bearer ${token}`);

			xmlHTTP.send();
		}
	}

	doesFileExist(urlToFile: string, token: string): boolean {
		const xhr = new XMLHttpRequest();
		xhr.open("HEAD", urlToFile, false);
		xhr.setRequestHeader("Authorization", `Bearer ${token}`);
		xhr.send();
		if (xhr.status === 404) {
			return false;
		}
		return true;
	}

	deleteAllFiles(employeeEntityTypeId: number, idEmployee: number, fileId: number): Promise<any> {
		return new Promise((resolve, reject) => {
			this.fileManagementService
				.deleteFile(employeeEntityTypeId, idEmployee, fileId, this.uploadFileType.file)
				.then(() => {
					this.snackBarService.openSnackBar({
						message: "deletedSuccessfully",
						icon: true,
						action: null,
						secondsDuration: 10
					});
					return resolve("ok");
				})
				.catch((error) => {
					this.alertService.error(error.error.Error, "errorFiles");
					return reject();
				});
		});
	}

	transformUrlToBinaryContent(file: any, extension: string, zip: JSZip): Promise<any> {
		return new Promise((resolve, reject) => {
			try {
				const xmlHTTP = new XMLHttpRequest();
				xmlHTTP.open("GET", file.url, true);
				xmlHTTP.responseType = "blob";
				xmlHTTP.onreadystatechange = () => {
					const promiseReady = 4;
					const statusOk = 200;
					if (xmlHTTP.readyState === promiseReady && xmlHTTP.status === statusOk) {
						zip.file(`${file.description}-${file.fileTypeDescription}-${Date.now()}.${extension}`, xmlHTTP.response);
						return resolve("ok");
					}
				};

				xmlHTTP.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);
				xmlHTTP.send();
			} catch (error) {
				return reject(error);
			}
		});
	}

	downloadAll(): void {
		const promises: Array<Promise<any>> = [];
		const zip = new JSZip();
		this.files.forEach((element) => {
			const extension = element.urlName.split(".").pop();
			if (this.fileType[extension] === this.uploadFileType.image) {
				promises.push(this.toDataURL(element.file, extension, zip));
			} else {
				promises.push(this.transformUrlToBinaryContent(element.file, extension, zip));
			}
		});

		if (promises.length > 0) {
			Promise.all(promises)
				.then(() => {
					zip.generateAsync({ type: "blob" })
						.then(function (content: any): void {
							FileSaver.saveAs(content, "EmployeeFiles.zip");
						})
						.catch((error) => {
							this.alertService.error(error.error.Error, "errorFiles");
						});
				})
				.catch((error) => {
					this.alertService.error(error.error.Error, "errorFiles");
				});
		} else {
			zip.generateAsync({ type: "blob" })
				.then(function (content: any): void {
					FileSaver.saveAs(content, "EmployeeFiles.zip");
				})
				.catch((error) => {
					this.alertService.error(error.error.Error, "errorFiles");
				});
		}
	}

	toDataURL(file: any, extension: string, zip: JSZip): Promise<any> {
		return new Promise((resolve, reject) => {
			try {
				const xmlHTTP = new XMLHttpRequest();
				xmlHTTP.open("GET", file.url, true);
				xmlHTTP.responseType = "arraybuffer";
				xmlHTTP.onreadystatechange = () => {
					const promiseReady = 4;
					const statusOk = 200;
					if (xmlHTTP.readyState === promiseReady && xmlHTTP.status === statusOk) {
						zip.file(`${file.description}-${file.fileTypeDescription}-${Date.now()}.${extension}`, xmlHTTP.response);
						return resolve("ok");
					}
				};
				xmlHTTP.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);
				xmlHTTP.send();
			} catch (error) {
				return reject(error);
			}
		});
	}

	// view(fileId: number): void {
	// 	const file = this.files.find((x) => x.id === fileId);
	// 	const extension = file.urlName.split(".").pop();

	// 	if (this.fileType[extension] === this.uploadFileType.image) {
	// 		this.fileTypeExtension = this.uploadFileType.image;
	// 	} else {
	// 		this.fileTypeExtension = this.uploadFileType.file;
	// 	}
	// 	this.dialog.open(ModalImagePreviewComponent, {
	// 		disableClose: true,
	// 		hasBackdrop: true,
	// 		width: "70%",
	// 		height: "70%",
	// 		data: {
	// 			fileTypeDescription: file.file.fileTypeDescription,
	// 			description: file.file.description,
	// 			extension: extension,
	// 			dataImage: file.dataImage
	// 		}
	// 	});
	// }

	view(fileId: number): void {
		const file = this.files.find((x) => x.file.fileId === fileId);
		const item = [];
		item.push({
			dropFileType: this.dragDropFileService.checkFileType(file.urlName),
			file: file,
			isUpdate: true,
			fromView: true
		});

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
}
