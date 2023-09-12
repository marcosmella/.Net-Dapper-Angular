import { Injectable, ApplicationRef, EventEmitter } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { finalize } from "rxjs/operators";

import { LoadingService } from "./loading.service";
import { AppConfigService } from "./app.config.service";

export interface LangChangeEvent {
	lang: string;
	dictionary: any;
}

export interface TranslationChangeEvent {
	lang: string;
	dictionary: any;
}

@Injectable({
	providedIn: "root"
})
export class TranslateService {
	public dictionary: any = {};
	public lang: any = { label: "es-AR" };
	private _onLangChange: EventEmitter<LangChangeEvent> = new EventEmitter<LangChangeEvent>();
	private _onTranslationChange: EventEmitter<TranslationChangeEvent> = new EventEmitter<TranslationChangeEvent>();

	constructor(private http: HttpClient, private conf: AppConfigService, private loading: LoadingService, private ar: ApplicationRef) {
		const setLang = localStorage.getItem("lang");
		const langTimeStorage = localStorage.getItem("langTime");

		if (setLang && langTimeStorage) {
			const langTime = JSON.parse(langTimeStorage).timestamp;
			const currentTime = new Date().getTime();
			const subtraction = currentTime - langTime;
			if (Math.round(subtraction / (1000 * 60 * 60)) > 12) {
				this.changeLang(this.lang);
			} else {
				this.lang = setLang;
				this.dictionary = JSON.parse(localStorage.getItem("dictionary"));
			}
		} else {
			this.lang = { label: "es-AR" };
			this.changeLang(this.lang);
		}
	}
	get onTranslationChange(): EventEmitter<TranslationChangeEvent> {
		return this._onTranslationChange;
	}
	get onLangChange(): EventEmitter<LangChangeEvent> {
		return this._onLangChange;
	}

	getLangs(): any[] {
		return [{ label: "es-AR" }, { label: "en-US" }, { label: "no-NO" }];
	}

	changeLang(lang: any): void {
		this.lang = lang;
		localStorage.setItem("lang", this.lang);
		this.loading.show("fs");
		this.http
			.get(`${this.conf.apiEndpoint}Tags/api/1/Tags/${this.lang.label}`)
			.pipe(finalize(() => this.loading.hide("fs")))
			.subscribe(
				labels => {
					this.dictionary = null;
					this.dictionary = labels;
					this.onLangChange.emit({ lang: lang, dictionary: this.dictionary });
					localStorage.removeItem("dictionary");
					localStorage.setItem("dictionary", JSON.stringify(this.dictionary));
					localStorage.setItem("langTime", JSON.stringify({ value: "value", timestamp: new Date().getTime() }));
					setTimeout(() => {
						this.ar.tick();
					}, 100);
					window.location.reload(true);
				},
				() => {}
			);
	}
}
