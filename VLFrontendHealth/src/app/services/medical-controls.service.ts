import { HttpClient } from "@angular/common/http";
import { EventEmitter, Injectable } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { Subject } from "rxjs/internal/Subject";
import { Observable } from "rxjs/internal/Observable";

import { MedicalControl } from "../models/medical-control.model";
import { AppConfigService } from "./app.config.service";
import { Patch } from "../models/patch.model";
import { AbsenceType } from "./../models/absence-type.model";

@Injectable({
	providedIn: "root"
})
export class MedicalControlsService {
	public medicalControlData = new MedicalControl();
	public actionForm: FormGroup;
	public filterAbsenceTypeWhenAbsenceExtension$ = new Subject<number>();
	public filterAbsenceTypeWhenReopening$ = new Subject<number>();
	public getAbsenceTypes$ = new Subject<number>();
	public workAccidentControlType$ = new Subject<boolean>();
	public isPathology$ = new Subject<boolean>();
	public onLoadActionAbsence = new EventEmitter();
	public absenceTypes: AbsenceType[];
	public idTrackingTypeSelected: number;
	public isMedicalControlAccident: boolean;
	public idAbsenceTypeParent: number;
	public idControlTypeParent = 0;
	private actionForm$ = new Subject<FormGroup>();

	private _url: string;
	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api/medical-controls`;
		this.idTrackingTypeSelected = 0;
		this.isMedicalControlAccident = false;
		this.idAbsenceTypeParent = 0;
	}

	public get(id: number, tracking: boolean = false): Observable<MedicalControl> {
		return this._http.get<MedicalControl>(`${this._url}/${id}?tracking=${tracking}`);
	}

	public post(medicalControl: MedicalControl): Observable<any> {
		return this._http.post<MedicalControl>(`${this._url}`, medicalControl);
	}

	public put(medicalControl: MedicalControl): Observable<any> {
		return this._http.put<MedicalControl>(`${this._url}`, medicalControl);
	}

	public delete(idMedicalControl: number): Observable<any> {
		return this._http.delete(`${this._url}/${idMedicalControl}`);
	}

	public patch(idMedicalControl: number, patchData: Patch[]): Observable<any> {
		return this._http.patch(`${this._url}/${idMedicalControl}`, patchData);
	}

	setForm(data: MedicalControl): void {
		this.actionForm.reset(data);
		this.actionForm$.next(this.actionForm);
	}
	setAbsenceForm(data: MedicalControl): void {
		if (this.actionForm) {
			this.actionForm.reset(data.absence);
			this.actionForm$.next(this.actionForm);
		}
	}

	getActionForm$(): Observable<FormGroup> {
		return this.actionForm$.asObservable();
	}
}
