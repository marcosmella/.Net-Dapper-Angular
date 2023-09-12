import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { AbsenceFilterParameters, EmployeePagination, FilterParameters } from "../models/employee-filter.model";
import { AbsencePagination } from "../models/absence-filter.model";
import { MedicalControlList, MedicalControlsFilterParameters } from "../models/medical-control.model";

@Injectable({
	providedIn: "root"
})
export class SearchEngineFilterService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}search/api/search-engines/`;
	}

	public getEmployeeByFilter(employee: FilterParameters): Observable<EmployeePagination> {
		const parameter = this.getEmployeeParameters(employee);
		return this._http.get<EmployeePagination>(`${this._url}employees?${parameter}`);
	}

	public getAbsencesByFilter(employee: AbsenceFilterParameters): Observable<AbsencePagination> {
		const parameter = this.getAbsenceParameters(employee);
		return this._http.get<AbsencePagination>(`${this._url}absences?${parameter}`);
	}

	public getMedicalControlByFilter(medicalControlFilter: MedicalControlsFilterParameters): Observable<MedicalControlList> {
		const parameter = this.getMedicalControlParameters(medicalControlFilter);
		return this._http.get<MedicalControlList>(`${this._url}medical-controls?${parameter}`);
	}

	getHttpObject(object: any): HttpParams {
		const form = new HttpParams({
			fromObject: object
		});
		return form;
	}

	getAbsenceParameters(employee: any): string {
		let queryParamsString = this.getHttpObject(employee);

		if (employee.idAbsenceType === 0) {
			queryParamsString = queryParamsString.set("idAbsenceType", "");
		}

		if (employee.absenceStatus === 0) {
			queryParamsString = queryParamsString.set("absenceStatus", "");
		}
		if (employee.absencePeriodRange) {
			const dateFrom = employee.absencePeriodRange.start;
			const dateTo = employee.absencePeriodRange.end;

			if (dateFrom) {
				queryParamsString = queryParamsString.set("AbsenceRange.Start", new Date(dateFrom).toISOString());
			}
			if (dateTo) {
				queryParamsString = queryParamsString.set("AbsenceRange.End", new Date(dateTo).toISOString());
			}
		}
		return queryParamsString.toString();
	}

	getEmployeeParameters(employee: any): string {
		let queryParamsString = new HttpParams({
			fromObject: employee
		});

		if (employee.hireRange) {
			if (employee.hireRange.hireDateFrom) {
				queryParamsString = queryParamsString.set("Entry.Start", new Date(employee.hireRange.hireDateFrom).toISOString());
			}
			if (employee.hireRange.hireDateTo) {
				queryParamsString = queryParamsString.set("Entry.End", new Date(employee.hireRange.hireDateTo).toISOString());
			}
		}
		if (employee.terminationRange) {
			if (employee.terminationRange.terminationDateFrom) {
				queryParamsString = queryParamsString.set("Egress.Start", new Date(employee.terminationRange.terminationDateFrom).toISOString());
			}

			if (employee.terminationRange.terminationDateTo) {
				queryParamsString = queryParamsString.set("Egress.End", new Date(employee.terminationRange.terminationDateTo).toISOString());
			}
		}
		if (employee.structure) {
			employee.structure.forEach((element, index) => {
				if (element.idStructureType || element.idStructure) {
					queryParamsString = queryParamsString.set(`structures[${index}].IdStructureType`, element.idStructureType);
					queryParamsString = queryParamsString.set(`structures[${index}].IdStructure`, element.idStructure);
					if (element.start !== undefined) {
						queryParamsString = queryParamsString.set(`structures[${index}].Start`, element.start);
					}
				}
			});
		}
		return queryParamsString.toString();
	}

	getMedicalControlParameters(medicalControlFilter: MedicalControlsFilterParameters): string {
		let queryParamsString = new HttpParams();
		if (medicalControlFilter.orderBy) {
			queryParamsString = queryParamsString.set("orderBy", medicalControlFilter.orderBy);
		}
		if (medicalControlFilter.page) {
			queryParamsString = queryParamsString.set("page", medicalControlFilter.page.toString());
		}
		if (medicalControlFilter.pageSize) {
			queryParamsString = queryParamsString.set("pageSize", medicalControlFilter.pageSize.toString());
		}
		if (medicalControlFilter.fileNumber) {
			queryParamsString = queryParamsString.set("fileNumber", medicalControlFilter.fileNumber);
		}
		if (medicalControlFilter.name) {
			queryParamsString = queryParamsString.set("Name", medicalControlFilter.name);
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
		if (medicalControlFilter.idMedicalControlAction) {
			queryParamsString = queryParamsString.set("idMedicalControlAction", medicalControlFilter.idMedicalControlAction.toString());
		}
		// tslint:disable-next-line: no-collapsible-if
		if (medicalControlFilter.structure) {
			if (medicalControlFilter.structure.idStructureType || medicalControlFilter.structure.idStructure) {
				queryParamsString = queryParamsString.set(
					`structures[0].IdStructureType`,
					medicalControlFilter.structure.idStructureType.toString()
				);
				queryParamsString = queryParamsString.set(
					`structures[0].IdStructure`,
					medicalControlFilter.structure.idStructure > 0 ? medicalControlFilter.structure.idStructure?.toString() : ""
				);
				if (medicalControlFilter.structure.start !== undefined) {
					queryParamsString = queryParamsString.set(`structures[0].Start`, medicalControlFilter.structure.start.toString());
				}
			}
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
