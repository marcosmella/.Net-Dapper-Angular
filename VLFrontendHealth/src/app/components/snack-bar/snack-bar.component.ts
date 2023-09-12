import { Component, OnInit, Inject, ViewEncapsulation } from "@angular/core";
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from "@angular/material/snack-bar";

@Component({
	selector: "app-snack-bar",
	templateUrl: "./snack-bar.component.html",
	styleUrls: [],
	encapsulation: ViewEncapsulation.None
})
export class SnackBarComponent implements OnInit {
	constructor(public snackBarRef: MatSnackBarRef<SnackBarComponent>, @Inject(MAT_SNACK_BAR_DATA) public data: any) {}

	ngOnInit(): void {}
}
