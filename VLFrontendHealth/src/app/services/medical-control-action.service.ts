import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";

import { Select } from "../models/select.model";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class MedicalControlActionService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api/medical-controls/`;
	}

	public get(id: number): Observable<Select[]> {
		return this._http.get<Select[]>(`${this._url}control-types/${id}/actions`);
	}

	public getActions(): Observable<Select[]> {
		return this._http.get<Select[]>(`${this._url}actions`);
	}
}
