import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { Absence, AbsenceRequest } from "./../models/absence.model";
@Injectable({
	providedIn: "root"
})
export class AbsenceService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Absence/api/absences`;
	}

	public getById(id: number): Observable<AbsenceRequest> {
		return this._http.get<AbsenceRequest>(`${this._url}/${id}`);
	}

	public post(absence: Absence): Observable<number> {
		return this._http.post<number>(`${this._url}`, absence);
	}

	public reopening(absence: Absence): Observable<Array<number>> {
		return this._http.post<Array<number>>(`${this._url}/reopening`, absence);
	}

	public delete(id: number): Observable<any> {
		return this._http.delete(`${this._url}/${id}`);
	}

	public cancel(id: number): Observable<any> {
		return this._http.put(`${this._url}/${id}/cancel`, null);
	}
}
