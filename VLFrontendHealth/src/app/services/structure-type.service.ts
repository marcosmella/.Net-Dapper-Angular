import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { StructureType } from "../models/structure-type.model";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class StructureTypeService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Organization/api/structure-types`;
	}

	public create(structureType: StructureType): Observable<any> {
		return this._http.post(this._url, structureType);
	}

	public update(structureType: StructureType): Observable<any> {
		return this._http.put(this._url, structureType);
	}

	public delete(id: number): Observable<any> {
		return this._http.delete(`${this._url}/${id}`);
	}

	public get(): Observable<StructureType[]> {
		return this._http.get<StructureType[]>(this._url);
	}

	public getId(id: number): Observable<StructureType> {
		return this._http.get<StructureType>(`${this._url}/${id}`);
	}
}
