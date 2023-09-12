import { Injectable } from "@angular/core";
import { ValidatorFn, AbstractControl } from "@angular/forms";
import * as moment from "moment";

@Injectable({
	providedIn: "root"
})
export class DateValidators {
	constructor() {}
	dateLessThan(dateFrom: string, dateTo: string): ValidatorFn {
		return (control: AbstractControl): { [s: string]: string } => {
			if (
				control.parent &&
				control.parent.get(dateFrom).value &&
				control.parent.get(dateTo).value &&
				new Date(control.parent.get(dateFrom).value) > new Date(control.parent.get(dateTo).value)
			) {
				return {
					invalidDateRanges: "dateLessThan"
				};
			}
			return null;
		};
	}

	dateLessThanToday(dateFrom: string): ValidatorFn {
		const today = new Date();
		today.setDate(today.getDate());
		return (control: AbstractControl): { [s: string]: string } => {
			if (control.parent && control.parent.get(dateFrom).value && control.parent.get(dateFrom).value > today) {
				return {
					invalidDateRanges: "dateLessThanToday"
				};
			}
			return null;
		};
	}

	dateGreaterThan(date: Date): ValidatorFn {
		return (control: AbstractControl): { [s: string]: string } => {
			if (control.value && new Date(control.value) < new Date(date)) {
				return {
					invalidDateRanges: "dateGreaterThan"
				};
			}
		};
	}

	dateLessEqualThan(dateFrom: string, dateTo: string): ValidatorFn {
		return (control: AbstractControl): { [s: string]: string } => {
			let startDate: any;
			let endDate: any;

			if (control.parent && control.parent.get(dateFrom).value && control.parent.get(dateTo).value) {
				startDate = moment.utc(control.parent.get(dateFrom).value);
				endDate = moment.utc(control.parent.get(dateTo).value);
			}
			if (control.parent && control.parent.get(dateFrom).value && control.parent.get(dateTo).value && startDate.isSameOrAfter(endDate)) {
				return {
					invalidDateRanges: "dateLessEqualThan"
				};
			}
			return null;
		};
	}
}
