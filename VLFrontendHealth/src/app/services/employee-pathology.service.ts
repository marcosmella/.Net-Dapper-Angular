import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from ".//app.config.service";
import { Pathology } from "./../models/pathology.model";
import { EmployeePathologies } from "../models/employee-pathology.model";

@Injectable({
	providedIn: "root"
})
export class EmployeePathologyService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api/clinical-records/employees/`;
	}

	public get(idPerson: number): Observable<Pathology[]> {
		return this._http.get<Pathology[]>(`${this._url}${idPerson}/pathologies`);
	}

	public put(pathologies: EmployeePathologies): Observable<any> {
		return this._http.put(`${this._url}pathologies`, pathologies);
	}
}
