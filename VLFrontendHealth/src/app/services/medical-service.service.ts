import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";

import { MedicalService } from "../models/medical-service";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class MedicalServiceService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api`;
	}

	public get(): Observable<MedicalService[]> {
		return this._http.get<MedicalService[]>(`${this._url}/medical-services`);
	}

	public getById(idMedicalService: number): Observable<MedicalService> {
		return this._http.get<MedicalService>(`${this._url}/medical-services/${idMedicalService}`);
	}

	public put(medicalService: MedicalService): Observable<any> {
		return this._http.put(`${this._url}/medical-services`, medicalService);
	}

	public post(medicalService: MedicalService): Observable<any> {
		return this._http.post(`${this._url}/medical-services`, medicalService);
	}

	public delete(idMedicalService: number): Observable<any> {
		return this._http.delete(`${this._url}/medical-services/${idMedicalService}`);
	}
}
