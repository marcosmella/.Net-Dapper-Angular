import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";

import { AlertService } from "./../../services/alert.service";
import { DoctorService } from "./../../services/doctor.service";
import { SnackBarService } from "./../../services/snack-bar.service";
import { SuccessService } from "./../../services/success.service";
import { LoadingSpinnerComponent } from "../loading-spinner/loading-spinner.component";
import { Doctor } from "./../../models/doctors.model";
import { EnumFeedback } from "./../../models/enumFeedback.model";
import { TranslatePipe } from "./../../pipes/translate.pipe";
import { MenuService } from "../../services/menu.service";

@Component({
	selector: "app-doctor",
	templateUrl: "./doctor.component.html",
	styleUrls: ["./doctor.component.scss"]
})
export class DoctorComponent implements OnInit, AfterViewInit, OnDestroy {
	public formDoctor: FormGroup;
	@ViewChild("spinner", { static: false })
	spinner: LoadingSpinnerComponent;
	public isUpdate = false;

	constructor(
		private fb: FormBuilder,
		private doctorService: DoctorService,
		private snackBarService: SnackBarService,
		private alertService: AlertService,
		private router: Router,
		private route: ActivatedRoute,
		private successServ: SuccessService,
		private translatePipe: TranslatePipe,
		private menuService: MenuService
	) {
		this.formDoctor = this.fb.group({
			id: [0],
			firstName: [null, [Validators.required, Validators.maxLength(100)]],
			lastName: [null, [Validators.required, Validators.maxLength(100)]],
			enrollment: [null, [Validators.required, Validators.maxLength(15)]],
			enrollmentExpirationDate: [null, []],
			documentNumber: [null, [Validators.required, Validators.maxLength(25)]],
			documentExpirationDate: [null, []]
		});
	}

	ngOnInit(): void {
		this.menuService.hideMenu();
	}

	ngAfterViewInit(): void {
		this.route.paramMap.subscribe((params) => {
			const idDoctor = Number(params.get("id"));
			if (idDoctor) {
				this.getDoctorById(idDoctor);
			}
		});
	}

	ngOnDestroy(): void {
		this.menuService.showMenu();
	}

	getDoctorById(idDoctor: number): void {
		this.spinner.show();
		this.doctorService
			.getById(idDoctor)
			.subscribe(
				(data) => {
					this.isUpdate = true;
					this.formDoctor.reset(data);
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "doctorError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	update(doctor: Doctor): void {
		this.doctorService
			.put(doctor)
			.subscribe(
				() => {
					this.snackBarService.openSnackBar({
						message: `modifiedSuccessfully`,
						icon: true,
						secondsDuration: 5,
						action: null
					});

					this.router.navigateByUrl("/healthApp/doctors");
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "doctorError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	add(doctor: Doctor): void {
		this.doctorService
			.post(doctor)
			.subscribe(
				() => {
					this.feedbackOk();
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "doctorError");
					}
				}
			)
			.add(() => this.spinner.hide());
	}

	submit(): void {
		if (!this.formDoctor.valid) {
			this.formDoctor.markAllAsTouched();
			return;
		}

		this.spinner.show();
		if (this.isUpdate) {
			this.update(this.formDoctor.value);
		} else {
			this.add(this.formDoctor.value);
		}
	}

	feedbackOk(): void {
		this.successServ.clear();
		const textDescription = this.translatePipe.transform("successDoctorDescription");
		const textDoctorDescription = `"${this.formDoctor.value.firstName} ${this.formDoctor.value.lastName}"`;
		this.successServ.data.title = "createdSuccessfully";
		this.successServ.data.imageFeedBack = EnumFeedback.done;
		this.successServ.data.actionContinue = null;
		this.successServ.data.routeContinue = null;
		this.successServ.data.actionFinish = "finish";
		this.successServ.data.routeFinish = "/healthApp/doctors";
		this.successServ.data.description = `${textDescription} ${textDoctorDescription}`;
		this.successServ.data.optionalLastText = "";
		this.successServ.data.optionalActionText = "";
		this.successServ.data.optionalAction = null;
		this.router.navigateByUrl("/healthApp/success");
	}
}
