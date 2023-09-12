import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./../services/app.config.service";
import { FileType } from "./../models/file-type.model";

@Injectable({
	providedIn: "root"
})
export class UploadExamService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}GlobalConfiguration/api/1/`;
	}

	public get(): Observable<FileType[]> {
		return this._http.get<FileType[]>(`${this._url}file-types`);
	}
}
