import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { NoteType } from "../models/note-type.model";

@Injectable({
	providedIn: "root"
})
export class NoteTypeService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Person/api/note-types`;
	}

	public get(): Observable<NoteType[]> {
		return this._http.get<NoteType[]>(this._url);
	}

	public getById(idNoteType: number): Observable<NoteType> {
		return this._http.get<NoteType>(`${this._url}/${idNoteType}`);
	}

	public post(noteType: NoteType): Observable<any> {
		return this._http.post(this._url, noteType);
	}

	public put(noteType: NoteType): Observable<any> {
		return this._http.put(this._url, noteType);
	}

	public delete(idNoteType: number): Observable<any> {
		return this._http.delete(`${this._url}/${idNoteType}`);
	}
}
