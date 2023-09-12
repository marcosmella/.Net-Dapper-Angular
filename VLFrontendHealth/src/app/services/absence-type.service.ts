import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { HttpClient } from "@angular/common/http";

import { AbsenceType } from "../models/absence-type.model";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class AbsenceTypeService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Absence/api/absences-type`;
	}

	public get(idCountry: number = 0): Observable<AbsenceType[]> {
		let parameter = "";
		if (idCountry) {
			parameter = `?idCountry=${idCountry}`;
		}
		return this._http.get<AbsenceType[]>(`${this._url}${parameter}`);
	}

	public getById(id: number): Observable<AbsenceType> {
		return this._http.get<AbsenceType>(`${this._url}/${id}`);
	}

	public getByIdEmployeeAndDate(idEmployee: number, date: Date): Observable<AbsenceType[]> {
		return this._http.get<AbsenceType[]>(`${this._url}/${idEmployee}/${date.toISOString()}`);
	}
}
