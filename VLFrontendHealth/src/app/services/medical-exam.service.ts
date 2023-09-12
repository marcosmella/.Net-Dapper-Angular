import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";

import { MedicalExam } from "../models/medical-exam.model";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class MedicalExamService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api`;
	}

	public get(): Observable<MedicalExam[]> {
		return this._http.get<MedicalExam[]>(`${this._url}/medical-exams`);
	}

	public getById(idMedicalExam: number): Observable<MedicalExam> {
		return this._http.get<MedicalExam>(`${this._url}/medical-exams/${idMedicalExam}`);
	}

	public put(medicalExam: MedicalExam): Observable<any> {
		return this._http.put(`${this._url}/medical-exams`, medicalExam);
	}

	public post(medicalExam: MedicalExam): Observable<any> {
		return this._http.post(`${this._url}/medical-exams`, medicalExam);
	}

	public delete(idMedicalExam: number): Observable<any> {
		return this._http.delete(`${this._url}/medical-exams/${idMedicalExam}`);
	}
}
