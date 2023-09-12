import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { Router } from "@angular/router";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";

import { TranslatePipe } from "./../../pipes/translate.pipe";
import { Doctor } from "./../../models/doctors.model";
import { AlertService } from "./../../services/alert.service";
import { BreadCrumbService } from "./../../services/breadcrumb.service";
import { DoctorService } from "./../../services/doctor.service";
import { ExcelService } from "./../../services/excel.services";
import { ModalService } from "./../../services/modal.service";
import { SnackBarService } from "./../../services/snack-bar.service";
import { ElipsisAction, IconTypes } from "../elipsis-grid/elipsis-grid.component";
import { LoadingSpinnerComponent } from "../loading-spinner/loading-spinner.component";

@Component({
	selector: "app-doctors-list",
	templateUrl: "./doctors-list.component.html",
	styleUrls: ["./doctors-list.component.scss"]
})
export class DoctorsListComponent implements OnInit, AfterViewInit {
	@ViewChild("spinner", { static: false })
	public spinner: LoadingSpinnerComponent;
	@ViewChild(MatSort, { static: false })
	public sort: MatSort;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	public doctorsDisplayedColumns: string[] = ["id", "firstName", "lastName", "enrollment", "options"];
	public doctorsDataSource: MatTableDataSource<Doctor>;
	public actions: ElipsisAction[];
	public recordsCount: number;

	constructor(
		private doctorService: DoctorService,
		private router: Router,
		private alertService: AlertService,
		private modalService: ModalService,
		private snackBarService: SnackBarService,
		private breadCrumb: BreadCrumbService,
		private excelService: ExcelService,
		private translatePipe: TranslatePipe
	) {
		this.doctorsDataSource = new MatTableDataSource<Doctor>();
		this.actions = new Array<ElipsisAction>();
		this.actions.push(
			{
				action: (element) => this.edit(element.id),
				icon: IconTypes.edit
			},
			{
				action: (element) => this.delete(element.id),
				icon: IconTypes.delete
			}
		);
	}

	ngOnInit(): void {}

	ngAfterViewInit(): void {
		this.getDoctors();
		this.breadCrumb.showBreadCrumb([
			{ label: "hrCore", path: "/hrcore" },
			{ label: "health", path: "/healthApp" }
		]);
	}

	getDoctors(): void {
		this.spinner.show();
		this.doctorService
			.get()
			.subscribe(
				(doctors) => {
					this.doctorsDataSource.data = doctors;
					this.doctorsDataSource.paginator = this.paginator;
					this.doctorsDataSource.sort = this.sort;
					this.recordsCount = this.doctorsDataSource.data.length;
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "doctorsError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	create(): void {
		this.router.navigateByUrl(`/healthApp/doctors/create`);
	}

	edit(id: number): void {
		this.router.navigateByUrl(`/healthApp/doctors/modify/${id}`);
	}

	delete(id: number): void {
		this.modalService
			.openDialog({
				title: "atention",
				message: "areYouSureForDelete",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.spinner.show();
					this.doctorService
						.delete(id)
						.subscribe(
							() => {
								this.snackBarService.openSnackBar({
									message: "deletedSuccessfully",
									icon: true,
									action: null,
									secondsDuration: 5
								});
								this.getDoctors();
							},
							(error) => {
								this.alertService.error(error.error.Error, "doctorsError");
							}
						)
						.add(() => {
							this.spinner.hide();
						});
				}
			});
	}

	applyFilter(filterValue: string): void {
		this.doctorsDataSource.filter = filterValue.trim().toLowerCase();
		this.recordsCount = this.doctorsDataSource._filterData(this.doctorsDataSource.data).length;
	}

	exportAsXLSX(): void {
		let filteredDoctors = [];
		const filtered = this.doctorsDataSource._orderData(this.doctorsDataSource._filterData(this.doctorsDataSource.data));

		filteredDoctors = filtered.map((value) => ({
			id: value.id,
			firstName: value.firstName,
			lastName: value.lastName,
			enrollment: value.enrollment,
			enrollmentExpirationDate: value.enrollmentExpirationDate,
			documentNumber: value.documentNumber,
			documentExpirationDate: value.documentExpirationDate
		}));
		this.excelService.exportAsExcelFile(filteredDoctors, this.translatePipe.transform("doctors"));
	}
}
