import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { Select } from "../models/select.model";

@Injectable({
	providedIn: "root"
})
export class BloodTypeService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api/`;
	}

	public get(): Observable<Select[]> {
		return this._http.get<Select[]>(`${this._url}blood-types`);
	}
}
