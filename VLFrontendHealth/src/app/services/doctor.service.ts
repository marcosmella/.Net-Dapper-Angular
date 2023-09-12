import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";

import { Doctor } from "../models/doctors.model";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class DoctorService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api`;
	}

	public get(): Observable<Doctor[]> {
		return this._http.get<Doctor[]>(`${this._url}/doctors`);
	}

	public getById(idDoctor: number): Observable<Doctor> {
		return this._http.get<Doctor>(`${this._url}/doctors/${idDoctor}`);
	}

	public put(doctor: Doctor): Observable<any> {
		return this._http.put(`${this._url}/doctors`, doctor);
	}

	public post(doctor: Doctor): Observable<any> {
		return this._http.post(`${this._url}/doctors`, doctor);
	}

	public delete(idDoctor: number): Observable<any> {
		return this._http.delete(`${this._url}/doctors/${idDoctor}`);
	}
}
