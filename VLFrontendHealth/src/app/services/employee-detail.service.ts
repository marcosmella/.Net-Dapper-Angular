import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { Summary } from "../models/summary.model";
import { Employee } from "../models/employee.model";
import { AppConfigService } from "./app.config.service";
import { PersonService } from "./person.service";
import { DirectReport } from "../models/direct-report.model";

@Injectable({
	providedIn: "root"
})
export class EmployeeDetailService {
	public summary: Summary;
	public employee: Employee = new Employee();
	public directReport: DirectReport = new DirectReport();
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService, private personService: PersonService) {
		this._url = `${this.conf.apiEndpoint}Person/api/`;
		this.summary = new Summary();
		this.summary.active = false;
	}

	public getEmployee(idPerson: number): Observable<Employee> {
		return this._http.get<Employee>(`${this._url}employees/${idPerson}`);
	}

	public setEmployee(idPerson: number): void {
		this._http.get<Employee>(`${this._url}employees/${idPerson}`).subscribe((data) => {
			this.employee = data;
			this.summary.fileNumber = this.employee.fileNumber;
			this.setEmployeeFullName(idPerson);
			this.setDirectReport(this.employee.idDirectReport);
		});
	}

	public clear(): void {
		this.summary = new Summary();
		this.summary.active = false;
		this.employee = null;
	}

	private setDirectReport(idPerson: number): void {
		if (!idPerson) {
			return;
		}
		this.personService.getById(idPerson).subscribe((data: any) => {
			this.directReport = data;
			if (this.directReport) {
				this.summary.directReportName = `${this.directReport.lastName ? this.directReport.lastName : ""}, ${
					this.directReport.name ? this.directReport.name : ""
				}`;
			}
		});
	}

	private setEmployeeFullName(idPerson: number): void {
		this.personService.getPersonById(idPerson).subscribe((data: any) => {
			if (data) {
				this.summary.firstName = this.setFullName(data);
			}
		});
	}

	private setFullName(data: any): string {
		return `${data.lastName ? data.lastName : ""}, ${data.name ? data.name : ""}`;
	}
}
