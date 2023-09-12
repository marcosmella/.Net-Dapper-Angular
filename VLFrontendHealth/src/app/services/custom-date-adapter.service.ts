// customDateAdapter.ts
import { Injectable } from "@angular/core";
import { MomentDateAdapter } from "@angular/material-moment-adapter";
import * as moment from "moment";

import { UserPreferenceService } from "./user-preference.service";

@Injectable()
export class CustomDateAdapter extends MomentDateAdapter {
	constructor(private userPreferenceService: UserPreferenceService) {
		super("en-US"); // set default locale
	}

	public format(date: moment.Moment): string {
		const format = this.userPreferenceService.getDateFormatApp();

		return date.locale("en-US").format(format);
	}

	public parse(date: any): moment.Moment {
		const result = moment(date, this.userPreferenceService.getDateFormatApp(), false).isValid();
		if (!result) {
			return moment(null);
		}

		return new Date().At0Time(new Date(moment(date, this.userPreferenceService.getDateFormatApp(), false).toISOString()));
	}

	createDate(year: number, month: number, date: number): moment.Moment {
		// Moment.js will create an invalid date if any of the components are out of bounds, but we
		// explicitly check each case so we can throw more descriptive errors.
		if (month < 0 || month > 11) {
			throw Error(`Invalid month index "${month}". Month index has to be between 0 and 11.`);
		}

		if (date < 1) {
			throw Error(`Invalid date "${date}". Date has to be greater than 0.`);
		}

		const result = moment.utc({ year, month, date }).locale(this.locale);

		// If the result isn't valid, the date must have been out of bounds for this month.
		if (!result.isValid()) {
			throw Error(`Invalid date "${date}" for month with index "${month}".`);
		}
		return result;
	}
}
