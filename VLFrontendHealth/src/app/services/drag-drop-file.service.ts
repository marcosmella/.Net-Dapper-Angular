import { Injectable } from "@angular/core";

import { UploadFileParameter } from "./../models/upload-file-parameter.model";
import { AuthService } from "./auth.service";
import { FileManagementService } from "./file-management.service";

@Injectable({
	providedIn: "root"
})
export class DragDropFileService {
	public fileFormats = new Array("application/pdf", "image/png", "image/jpg", "image/jpeg", ".odt");
	public files = [];
	private maxSize = 10485760;
	private imageNotFound = "/assets/img/avatar-empty.png";
	private promiseReady = 4;
	private statusOk = 200;

	constructor(private fileManagementService: FileManagementService, private authServ: AuthService) {}

	isFileAllowed(fileName: string, acceptFormats: string[]): boolean {
		const extension = this.getExtensionFile(fileName);
		return acceptFormats.some((x) => x === extension);
	}

	allowedMaxFileSize(fileSize: number): boolean {
		return !(fileSize > this.maxSize);
	}

	setPostParameters(acceptParams: UploadFileParameter): Promise<any> {
		return new Promise((resolve, reject) => {
			this.fileManagementService
				.postFile(
					acceptParams.fileToUpload,
					acceptParams.entityTypeId,
					acceptParams.entityId,
					acceptParams.fileTypeId,
					acceptParams.fileType,
					acceptParams.active
				)
				.then(
					(data) => {
						if (data) {
							return resolve(JSON.parse(data.response));
						}
					},
					(error) => {
						return reject(error);
					}
				);
		});
	}

	setUpdateParameters(fileId: number, acceptParams: UploadFileParameter): Promise<any> {
		return new Promise((resolve, reject) => {
			this.fileManagementService
				.putFile(
					acceptParams.fileToUpload,
					acceptParams.entityTypeId,
					acceptParams.entityId,
					acceptParams.fileTypeId,
					acceptParams.fileType,
					acceptParams.active,
					fileId
				)
				.then(
					(data) => {
						if (data) {
							return resolve(JSON.parse(data.response));
						}
					},
					(error) => {
						return reject(error);
					}
				);
		});
	}

	checkFileType(filename: string): string {
		const extensionFile = this.getExtensionFile(filename);
		let fileType: string;
		const files = [".pdf", ".doc", ".docx"];
		const images = [".jpg", ".jpeg", ".png"];
		files.forEach((item) => {
			if (extensionFile === item) {
				fileType = "file";
			}
		});
		images.forEach((item) => {
			if (extensionFile === item) {
				fileType = "image";
			}
		});
		return fileType;
	}

	getExtensionFile(fileName: string): string {
		const regex = /(?:\.([^.]+))?$/;
		const extension = regex.exec(fileName);
		return extension[0];
	}

	getfileById(fileId: number, acceptParams: UploadFileParameter): Promise<any> {
		return new Promise((resolve, reject) => {
			this.fileManagementService
				.getfile(
					acceptParams.entityTypeId,
					acceptParams.entityId,
					acceptParams.fileTypeId,
					acceptParams.active,
					acceptParams.fileType,
					fileId
				)
				.subscribe(
					(data) => {
						if (data.totalCount > 0) {
							return resolve(data.values[0]);
						}
					},
					(error) => {
						return reject(error);
					}
				);
		});
	}

	deleteFile(fileId: number, acceptParams: any): Promise<any> {
		return new Promise((resolve, reject) => {
			this.fileManagementService.deleteFile(acceptParams.entityTypeId, acceptParams.file.entityId, fileId, acceptParams.file.type).then(
				(data) => {
					if (data) {
						return resolve(JSON.parse(data.response));
					}
				},
				(error) => {
					return reject(error);
				}
			);
		});
	}

	getFullFileName(file: any): string {
		const extension = file.iconClass.split("-")[1];
		return `${file.file.description}.${extension}`;
	}

	download(fileData: any): void {
		this.files = [];
		const extension = fileData.iconClass.split("-")[1];
		const fileDataName = this.getFullFileName(fileData);

		if (this.checkFileType(fileDataName) === "image") {
			this.files.push({ fileData: fileData, fileExtension: extension, fileName: fileDataName });
			this.downloadImage(this.files);
		} else {
			this.files.push({ fileData: fileData, fileExtension: extension, fileName: fileDataName });
			this.downloadDocument(this.files);
		}
	}

	downloadImage(fileData: any): void {
		const url = fileData[0].fileData.url;
		const image = this.getDataImage(url, this.authServ.loadToken(), fileData[0].fileName, fileData[0].fileExtension);
		window.open(image, "_self");
	}

	getDataImage(url: string, token: any, description: string, extension: string): any {
		const xmlHTTP = new XMLHttpRequest();
		let dataURL: any;
		xmlHTTP.open("GET", url, true);
		xmlHTTP.responseType = "arraybuffer";
		xmlHTTP.onreadystatechange = () => {
			if (xmlHTTP.readyState === this.promiseReady && xmlHTTP.status === this.statusOk) {
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

	getFileName(url: string): string {
		const urlSplit = url.split("/");
		return urlSplit[urlSplit.length - 1];
	}

	viewImage(url: string, token: any, extensionFile: string): any {
		token = {};

		const xmlHTTP = new XMLHttpRequest();

		return new Promise((resolve, reject) => {
			token.cancel = () => {
				xmlHTTP.abort();
				reject(new Error("Cancelled"));
			};

			let dataURL: any;

			xmlHTTP.open("GET", url, true);

			xmlHTTP.responseType = "arraybuffer";

			xmlHTTP.onreadystatechange = () => {
				if (xmlHTTP.readyState === this.promiseReady) {
					if (xmlHTTP.status === this.statusOk) {
						const b64 = btoa(
							new Uint8Array(xmlHTTP.response).reduce(function (data: any, byte: any): any {
								return data + String.fromCharCode(byte);
							}, "")
						);
						dataURL = `data:image/${extensionFile};base64,${b64}`;
						resolve(dataURL);
					} else {
						resolve(this.imageNotFound);
					}
				}
			};
			xmlHTTP.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);

			xmlHTTP.send();
		});
	}

	viewFile(url: string, token: any, extensionFile: string): any {
		token = {};

		const xmlHTTP = new XMLHttpRequest();

		return new Promise((resolve, reject) => {
			token.cancel = () => {
				xmlHTTP.abort();
				reject(new Error("Cancelled"));
			};

			let dataURL: any;

			xmlHTTP.open("GET", url, true);

			xmlHTTP.responseType = "arraybuffer";

			xmlHTTP.onreadystatechange = () => {
				if (xmlHTTP.readyState === this.promiseReady) {
					if (xmlHTTP.status === this.statusOk) {
						const b64 = btoa(
							new Uint8Array(xmlHTTP.response).reduce(function (data: any, byte: any): any {
								return data + String.fromCharCode(byte);
							}, "")
						);
						dataURL = `data:application/${extensionFile};base64,${b64}`;
						resolve(dataURL);
					} else {
						resolve(this.imageNotFound);
					}
				}
			};
			xmlHTTP.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);

			xmlHTTP.send();
		});
	}

	downloadDocument(fileData: any[]): void {
		this.getDataFile(fileData[0].fileData.file);
	}

	getDataFile(file: any): void {
		const xmlHTTP = new XMLHttpRequest();
		xmlHTTP.open("GET", file.url, true);
		xmlHTTP.responseType = "blob";
		xmlHTTP.onreadystatechange = () => {
			if (xmlHTTP.readyState === this.promiseReady && xmlHTTP.status === this.statusOk) {
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

	getFileIcon(fileName: string): string {
		const regex = /(?:\.([^.]+))?$/;
		const extension = regex.exec(fileName);
		return extension[0].split(".")[1];
	}
}
