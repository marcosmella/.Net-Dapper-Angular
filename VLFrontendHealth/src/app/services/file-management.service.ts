import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { AuthService } from "./auth.service";

@Injectable({
	providedIn: "root"
})
export class FileManagementService {
	public image64: any;
	private _url;
	private imageNotFound = "/assets/img/avatar-empty.png";
	constructor(private _http: HttpClient, private conf: AppConfigService, public authServ: AuthService) {
		this._url = `${this.conf.apiEndpoint}WebApi/fileManagement`;
	}

	postFile(
		fileToUpload: File,
		entityTypeId: Number,
		idEntity: Number,
		fileTypeId: Number,
		fileType: string = "IMAGE",
		active: boolean = true
	): any {
		const formData: FormData = new FormData();

		if (fileToUpload) {
			const fileImportName = fileToUpload.name.split(".")[0];
			formData.append(fileType, fileToUpload, fileToUpload.name);

			const xhr = new XMLHttpRequest();

			return new Promise((resolve) => {
				xhr.onload = () => {
					resolve(xhr);
				};

				xhr.open(
					"POST",
					// tslint:disable-next-line:max-line-length
					`${this._url}/files?entityTypeId=${entityTypeId}&entityId=${idEntity}&fileTypeId=${fileTypeId}&type=${fileType}&active=${active}&description=${fileImportName}`,
					true
				);

				xhr.setRequestHeader("X-Tenant-Id", `${this.authServ.loadTenant().Id}`);
				xhr.setRequestHeader("Ocp-Apim-Subscription-Key", !this.conf.load ? this.conf.ocpApiSubscriptionKey : "");
				xhr.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);

				xhr.send(formData);
			});
		}
	}

	getfile(
		entityTypeId: number,
		idEntity: number,
		fileTypeId: number,
		active: boolean = true,
		type: string = "",
		fileId: number = null
	): Observable<any> {
		let url = `${this._url}/files?entityTypeId=${entityTypeId}&entityId=${idEntity}&fileTypeId=${fileTypeId}`;
		if (fileId) {
			url += `&fileId=${fileId}`;
		}
		if (type) {
			url += `&type=${type}`;
		}
		if (active) {
			url += `&active=${active}`;
		}

		return this._http.get<any>(url);
	}

	getImportFile(entityTypeId: Number, idEntity: Number, fileTypeId: Number = null, fileId: number = null): Observable<any> {
		let param = "";
		if (fileId) {
			param += `&fileId=${fileId}`;
		}
		if (fileTypeId) {
			param += `&fileTypeId=${fileTypeId}`;
		}
		return this._http.get<any>(
			`${this._url}/files?entityTypeId=${entityTypeId}&entityId=${idEntity}&fileTypeId=${fileTypeId}&type=File${param}`
		);
	}

	viewImage(url: string, token: any): any {
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
				const promiseReady = 4;
				const statusOk = 200;
				if (xmlHTTP.readyState === promiseReady) {
					if (xmlHTTP.status === statusOk) {
						const b64 = btoa(
							new Uint8Array(xmlHTTP.response).reduce(function (data: any, byte: any): any {
								return data + String.fromCharCode(byte);
							}, "")
						);
						dataURL = `data:image/jpeg;base64,${b64}`;
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

	deleteFile(entityTypeId: Number, entityId: Number, fileId: Number, type: string = null): any {
		if (type) {
			type = "file";
		} else {
			type = "IMAGE";
		}

		const xhr = new XMLHttpRequest();
		return new Promise((resolve) => {
			xhr.onload = () => {
				resolve(xhr);
			};

			xhr.open("DELETE", `${this._url}/files`, true);
			xhr.setRequestHeader("Content-Type", "application/json");
			xhr.setRequestHeader("X-Tenant-Id", `${this.authServ.loadTenant().Id}`);
			xhr.setRequestHeader("Ocp-Apim-Subscription-Key", !this.conf.load ? this.conf.ocpApiSubscriptionKey : "");
			xhr.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);

			xhr.send(JSON.stringify({ EntityTypeId: entityTypeId, EntityId: entityId, FileId: fileId, Type: type }));
		});
	}

	putFile(
		fileToUpload: File,
		entityTypeId: Number,
		idEntity: Number,
		fileTypeId: Number,
		fileType: string = "IMAGE",
		active: boolean = true,
		fileId: number
	): any {
		const formData: FormData = new FormData();
		const fileImportName = fileToUpload.name.split(".")[0];
		formData.append(fileType, fileToUpload, fileToUpload.name);

		const xhr = new XMLHttpRequest();

		return new Promise((resolve) => {
			xhr.onload = () => {
				resolve(xhr);
			};

			let url = `${this._url}/files?entityTypeId=${entityTypeId}&entityId=${idEntity}&fileTypeId=${fileTypeId}`;
			if (fileId) {
				url += `&FileId=${fileId}`;
			}
			if (fileType) {
				url += `&type=${fileType}`;
			}
			if (active) {
				url += `&active=${active}`;
			}
			if (fileImportName) {
				url += `&description=${fileImportName}`;
			}

			xhr.open("PUT", url, true);

			xhr.setRequestHeader("X-Tenant-Id", `${this.authServ.loadTenant().Id}`);
			xhr.setRequestHeader("Ocp-Apim-Subscription-Key", !this.conf.load ? this.conf.ocpApiSubscriptionKey : "");
			xhr.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);

			xhr.send(formData);
		});
	}

	postTemplate(fileToUpload: File): any {
		const formData: FormData = new FormData();

		formData.append("Image", fileToUpload, fileToUpload.name);

		const xhr = new XMLHttpRequest();

		return new Promise((resolve) => {
			xhr.onload = () => {
				resolve(xhr);
			};

			xhr.open("POST", `${this._url}/template`, true);

			xhr.setRequestHeader("X-Tenant-Id", `${this.authServ.loadTenant().Id}`);
			xhr.setRequestHeader("Ocp-Apim-Subscription-Key", !this.conf.load ? this.conf.ocpApiSubscriptionKey : "");
			xhr.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);

			xhr.send(formData);
		});
	}

	postInterfaceFile(fileToUpload: File, idModel: number): any {
		const formData: FormData = new FormData();

		formData.append("file", fileToUpload, fileToUpload.name);

		const xhr = new XMLHttpRequest();

		return new Promise((resolve) => {
			xhr.onload = () => {
				resolve(xhr);
			};

			xhr.open("POST", `${this._url}/models/${idModel}/interface`, true);

			xhr.setRequestHeader("X-Tenant-Id", `${this.authServ.loadTenant().Id}`);
			xhr.setRequestHeader("Ocp-Apim-Subscription-Key", !this.conf.load ? this.conf.ocpApiSubscriptionKey : "");
			xhr.setRequestHeader("Authorization", `Bearer ${this.authServ.loadToken()}`);

			xhr.send(formData);
		});
	}
}
