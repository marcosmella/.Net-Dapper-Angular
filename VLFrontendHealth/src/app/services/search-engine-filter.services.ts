import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { MedicalControlList, MedicalControlsFilterParameters } from "../models/medical-control.model";

@Injectable({
	providedIn: "root"
})
export class SearchEngineFilterService {
	private _url: string;
	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}search/api/search-engines/`;
	}

	public getMedicalControlByFilter(medicalControlFilter: MedicalControlsFilterParameters): Observable<MedicalControlList> {
		const parameter = this.getMedicalControlParameters(medicalControlFilter);
		return this._http.get<MedicalControlList>(`${this._url}medical-controls?${parameter}`);
	}

	getMedicalControlParameters(medicalControlFilter: MedicalControlsFilterParameters): string {
		let queryParamsString = new HttpParams();

		if (medicalControlFilter.fileNumber) {
			queryParamsString = queryParamsString.set("fileNumber", medicalControlFilter.fileNumber);
		}
		if (medicalControlFilter.firstName) {
			queryParamsString = queryParamsString.set("firstName", medicalControlFilter.firstName);
		}
		if (medicalControlFilter.lastName) {
			queryParamsString = queryParamsString.set("lastName", medicalControlFilter.lastName);
		}
		if (medicalControlFilter.idControlType) {
			queryParamsString = queryParamsString.set("idControlType", medicalControlFilter.idControlType.toString());
		}
		if (medicalControlFilter.idAbsenceType) {
			queryParamsString = queryParamsString.set("idAbsenceType", medicalControlFilter.idAbsenceType.toString());
		}
		if (medicalControlFilter.medicalControlRange) {
			if (medicalControlFilter.medicalControlRange.start) {
				queryParamsString = queryParamsString.set(
					"MedicalControlRange.Start",
					new Date(medicalControlFilter.medicalControlRange.start).toISOString()
				);
			}
			if (medicalControlFilter.medicalControlRange.end) {
				queryParamsString = queryParamsString.set(
					"MedicalControlRange.End",
					new Date(medicalControlFilter.medicalControlRange.end).toISOString()
				);
			}
		}
		return queryParamsString.toString();
	}
}
