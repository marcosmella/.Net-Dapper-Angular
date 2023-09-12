import { AfterViewInit, Component, Input, OnInit } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { ActivatedRoute } from "@angular/router";

import { EmployeeService } from "../../services/employee.service";
import { TranslatePipe } from "../../pipes/translate.pipe";
import { BreadCrumbService } from "../../services/breadcrumb.service";
import { EmployeeDetailService } from "../../services/employee-detail.service";

@Component({
	selector: "app-medical-history",
	templateUrl: "./medical-history.component.html",
	styleUrls: ["./medical-history.component.scss"]
})
export class MedicalHistoryComponent implements OnInit, AfterViewInit {
	@Input() editing: boolean;
	public idEmployee = 0;

	constructor(
		private title: Title,
		private breadCrumb: BreadCrumbService,
		private translatePipe: TranslatePipe,
		private route: ActivatedRoute,
		public employeeDetailService: EmployeeDetailService,
		public employeeService: EmployeeService
	) {
		this.title.setTitle(`${this.translatePipe.transform("medicalHistory")} - Visma`);
	}

	ngOnInit(): void {
		this.route.paramMap.subscribe((params) => {
			this.idEmployee = Number(params.get("id"));
			if (this.idEmployee) {
				this.canIseeEmployee().then(() => {
					this.employeeDetail();
				});
			} else {
				this.editing = true;
			}
		});
	}

	ngAfterViewInit(): void {
		this.breadCrumb.showBreadCrumb([
			{ label: "hrCore", path: "/hrcore" },
			{ label: "health", path: "/healthApp" },
			{ label: "clinicalRecords", path: "healthApp/clinical-records" }
		]);
	}

	setEnableEditing(editing: boolean): void {
		this.editing = editing;
	}

	employeeDetail(): void {
		this.employeeDetailService.setEmployee(this.idEmployee);
		this.editing = false;
	}

	canIseeEmployee(): Promise<any> {
		return new Promise((resolve, reject) => {
			this.employeeService.get(this.idEmployee).subscribe(
				() => {
					return resolve("");
				},
				() => {
					return reject();
				}
			);
		});
	}
}
