import { Injectable, OnDestroy, Pipe, PipeTransform } from "@angular/core";
import { Subscription } from "rxjs/internal/Subscription";

import { TranslateService, LangChangeEvent, TranslationChangeEvent } from "../services/translate.service";

@Injectable({ providedIn: "root" })
@Pipe({
	name: "translate",
	pure: true
})
export class TranslatePipe implements PipeTransform, OnDestroy {
	value = "";
	lastKey: string;
	lastParams: any[];
	onLangChange: Subscription;
	onTranslationChange: Subscription;

	constructor(private translate: TranslateService) {}

	updateValue(key: string, translations?: any): void {
		const onTranslation = (res: string) => {
			this.value = res !== undefined ? res : key;
			this.lastKey = key;
		};
		if (translations) {
			const res = this.translate.dictionary[key];
			onTranslation(res);
		}
	}

	transform(query: string, ...args: any[]): any {
		if (!query || !query.length) {
			return query;
		}

		// if we ask another time for the same key, return the last value
		if (query === this.lastKey) {
			return this.value;
		}

		// store the query, in case it changes
		this.lastKey = query;

		// store the params, in case they change
		this.lastParams = args;

		// set the value
		this.updateValue(query, this.translate.dictionary);

		// if there is a subscription to onLangChange, clean it
		this._dispose();

		// subscribe to onLangChange event, in case the language changes
		if (!this.onLangChange) {
			this.onLangChange = this.translate.onLangChange.subscribe((event: LangChangeEvent) => {
				if (this.lastKey) {
					this.lastKey = null; // we want to make sure it doesn't return the same value until it's been updated
					this.updateValue(query, event.dictionary);
				}
			});
			// subscribe to onTranslationChange event, in case the translations change
			if (!this.onTranslationChange) {
				this.onTranslationChange = this.translate.onTranslationChange.subscribe((event: TranslationChangeEvent) => {
					if (this.lastKey && event.lang === this.translate.lang) {
						this.lastKey = null;
						this.updateValue(query, this.translate.dictionary);
					}
				});
			}
		}

		return this.value;
	}
	ngOnDestroy(): void {
		this._dispose();
	}
	/**
	 * Clean any existing subscription to change events
	 */
	private _dispose(): void {
		if (typeof this.onLangChange !== "undefined") {
			this.onLangChange.unsubscribe();
			this.onLangChange = undefined;
		}
		if (typeof this.onTranslationChange !== "undefined") {
			this.onTranslationChange.unsubscribe();
			this.onTranslationChange = undefined;
		}
	}
}
