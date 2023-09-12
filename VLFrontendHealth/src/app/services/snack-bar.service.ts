import { MatSnackBarRef } from "@angular/material/snack-bar";
import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";

import { SnackBarComponent } from "../components/snack-bar/snack-bar.component";

@Injectable({
	providedIn: "root"
})
export class SnackBarService {
	constructor(private _snackBar: MatSnackBar) {}

	openSnackBar({ message, action = "", icon = false, secondsDuration = 10 }: SnackBarData): MatSnackBarRef<SnackBarComponent> {
		return this._snackBar.openFromComponent(SnackBarComponent, {
			duration: secondsDuration * 1000,
			panelClass: "snackBar",
			data: { message, action, icon }
		});
	}
}
export interface SnackBarData {
	message: "modifiedSuccessfully" | "createdSuccessfully" | "deletedSuccessfully" | "savedSuccessfully";
	action: string;
	icon: boolean;
	secondsDuration: number;
}
