import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";

import { EmployeeMedicalHistory, EmployeeMedicalHistoryRequest } from "../models/employee-medical-history.model";

import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class EmployeeMedicalHistoryService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api/clinical-records/employees`;
	}

	public get(idPerson: number): Observable<EmployeeMedicalHistory> {
		return this._http.get<EmployeeMedicalHistory>(`${this._url}/${idPerson}/medical-history`);
	}

	public post(medicalHistory: EmployeeMedicalHistoryRequest): Observable<any> {
		return this._http.post<EmployeeMedicalHistoryRequest>(`${this._url}/medical-history`, medicalHistory);
	}

	public put(medicalHistory: EmployeeMedicalHistoryRequest): Observable<any> {
		return this._http.put<EmployeeMedicalHistoryRequest>(`${this._url}/medical-history`, medicalHistory);
	}
}
