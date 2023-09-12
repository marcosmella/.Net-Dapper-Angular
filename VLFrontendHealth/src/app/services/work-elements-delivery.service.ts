import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { take } from "rxjs/operators";

import { WorkElementDelivery } from "../models/employee-work-element.model";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class WorkElementsDeliveryService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}CompanyAssets/api/employees`;
	}

	getWorkElementsDeliveryByEmployee(idEmployee: number): Observable<WorkElementDelivery[]> {
		return this._http.get<WorkElementDelivery[]>(`${this._url}/${idEmployee}/work-elements-deliveries`).pipe(take(1));
	}
}
