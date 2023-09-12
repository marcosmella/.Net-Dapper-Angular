import { Pipe, PipeTransform } from "@angular/core";
import * as moment from "moment";

import { UserPreference } from "../models/user-preference.model";

@Pipe({
	name: "dateFormat",
	pure: true
})
export class DateFormatPipe implements PipeTransform {
	constructor(private userPreference: UserPreference) {}

	transform(value: string): string {
		if (!value) {
			return;
		}
		return moment(value).format(this.userPreference.dateFormat.format.toUpperCase());
	}
}
