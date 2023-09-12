import { Component, OnInit, Input, forwardRef, Output, EventEmitter } from "@angular/core";
import { FormGroup, FormBuilder, ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";
import { debounceTime, tap, switchMap, finalize, filter } from "rxjs/operators";

import { SearchEngineFilterService } from "../../services/search-engine-filter.service";
import { EmployeePagination, FilterEmployee } from "../../models/employee-filter.model";

@Component({
	selector: "app-select-employee-search",
	templateUrl: "./select-employee-search.component.html",
	styleUrls: ["./select-employee-search.component.scss"],
	providers: [
		{
			provide: NG_VALUE_ACCESSOR,
			useExisting: forwardRef(() => SelectEmployeeSearchComponent),
			multi: true
		}
	]
})
export class SelectEmployeeSearchComponent implements OnInit, ControlValueAccessor {
	@Input("clearInput") set lastName(value: boolean) {
		if (value) {
			this.formEmployeeSearch.get("lastName").setValue("");
			this.employees = [];
		}
	}
	public employees: FilterEmployee[];
	public formEmployeeSearch: FormGroup;
	public isLoading: boolean;
	public employeeSelected = "";

	@Input() editing: boolean;
	@Input() isRequired: Boolean;
	@Output() employee: EventEmitter<FilterEmployee> = new EventEmitter<FilterEmployee>();

	constructor(private employeeFilterService: SearchEngineFilterService, private fb: FormBuilder) {
		this.formEmployeeSearch = this.fb.group({
			name: [""],
			lastName: [""],
			fileNumber: [""],
			active: [true],
			hireRange: this.fb.group({
				hireDateFrom: [""],
				hireDateTo: [""]
			}),
			terminationRange: this.fb.group({
				terminationDateFrom: [""],
				terminationDateTo: [""]
			}),
			page: [0],
			pageSize: [500],
			orderBy: ["fileNumber DESC"]
		});
	}

	onChange = (_: FilterEmployee) => {};
	onTouch = () => {};

	ngOnInit(): void {
		this.autoComplete();
	}

	writeValue(): void {}

	registerOnChange(fn: any): void {
		this.onChange = fn;
	}

	registerOnTouched(fn: any): void {
		this.onTouch = fn;
	}

	autoComplete(): void {
		const minimumNumberOfCharacters = 3;
		this.formEmployeeSearch
			.get("lastName")
			.valueChanges.pipe(
				debounceTime(900),
				filter((x) => x.length >= minimumNumberOfCharacters && x.toString() !== this.employeeSelected),

				tap(() => {
					this.employeeSelected = "";
					this.employees = [];
					this.isLoading = true;
				}),
				switchMap(() =>
					this.employeeFilterService.getEmployeeByFilter(this.formEmployeeSearch.value).pipe(finalize(() => (this.isLoading = false)))
				)
			)
			.subscribe(
				(data: EmployeePagination) => {
					data.paginationList.forEach((employee) => {
						employee["description"] = this.setEmployeeDescription(employee);
					});
					this.employees = data.paginationList;
				},
				() => {
					this.onChange(null);
					this.employees = [];
					this.employeeSelected = "";
					this.autoComplete();
				}
			);

		this.formEmployeeSearch.get("lastName").valueChanges.subscribe((value) => {
			if (!value) {
				this.onChange(null);
			}
		});
	}

	setEmployeeDescription(employee: FilterEmployee): string {
		return `${employee.fileNumber} - ${employee.lastName}, ${employee.name}`;
	}

	selectEmployee(employee: FilterEmployee): void {
		this.employeeSelected = this.setEmployeeDescription(employee);
		this.employee.emit(employee);
		this.onChange(employee);
	}
}
