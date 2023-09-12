import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { EmployeePagination, FilterParameters } from "./../models/clinical-record-filter.model";

@Injectable({
	providedIn: "root"
})
export class ClinicalRecordFilterService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}search/api/search-engines/`;
	}

	public getClinicalRecordByFilter(employee: FilterParameters): Observable<EmployeePagination> {
		const parameter = this.getParameters(employee);
		return this._http.get<EmployeePagination>(`${this._url}employees?${parameter}`);
	}

	getParameters(employee: any): string {
		let queryParamsString = new HttpParams({
			fromObject: employee
		});

		if (employee.structure) {
			employee.structure.forEach((element, index) => {
				if (element.idStructureType || element.idStructure) {
					queryParamsString = queryParamsString.set(`structures[${index}].IdStructureType`, element.idStructureType);
					queryParamsString = queryParamsString.set(`structures[${index}].IdStructure`, element.idStructure);
				}
			});
		}

		return queryParamsString.toString();
	}
}
