import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { Select } from "../models/select.model";
import { Pathologies } from "./../models/pathology.model";

@Injectable({
	providedIn: "root"
})
export class PathologyService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api/`;
	}

	public getById(id: number): Observable<Pathologies> {
		return this._http.get<Pathologies>(`${this._url}pathologies/${id}`);
	}

	public get(pathologyFilter?: Select): Observable<Pathologies[]> {
		const parameter = this.getParameters(pathologyFilter);
		return this._http.get<Pathologies[]>(`${this._url}pathologies?${parameter}`);
	}

	public getAll(): Observable<Pathologies[]> {
		return this._http.get<Pathologies[]>(`${this._url}pathologies`);
	}

	getParameters(pathology: any): string {
		let queryParamsString = new HttpParams();

		if (pathology.pathologySearch) {
			queryParamsString = queryParamsString.set("pathologyFilter", String(pathology.pathologySearch));
		}
		return queryParamsString.toString();
	}
}
