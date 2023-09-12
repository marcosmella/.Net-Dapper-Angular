import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";

@Component({
	selector: "app-work-element-grid",
	templateUrl: "./work-element-grid.component.html",
	styleUrls: ["./work-element-grid.component.scss"]
})
export class WorkElementGridComponent implements OnInit {
	@Input() idEmployee: number;

	constructor(private router: Router) {}

	ngOnInit(): void {}

	goToElementss(): void {
		this.router.navigateByUrl(`/staff-administration/employee/edit/${this.idEmployee}`);
	}
}
