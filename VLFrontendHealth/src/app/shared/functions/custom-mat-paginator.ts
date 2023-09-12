import { Injectable, OnInit } from "@angular/core";
import { MatPaginatorIntl } from "@angular/material/paginator";

import { TranslatePipe } from "../../pipes/translate.pipe";

@Injectable()
export class CustomMatPaginatorIntl extends MatPaginatorIntl implements OnInit {
	private ofLabel = this.translatePipe.transform("of");

	constructor(private translatePipe: TranslatePipe) {
		super();
		this.setTranslations();
	}

	ngOnInit(): void {}

	setTranslations(): void {
		this.nextPageLabel = this.translatePipe.transform("nextPage");
		this.previousPageLabel = this.translatePipe.transform("previousPage");
		this.firstPageLabel = this.translatePipe.transform("firstPage");
		this.lastPageLabel = this.translatePipe.transform("lastPage");
		this.itemsPerPageLabel = this.translatePipe.transform("ItemsPerPage");
	}

	getRangeLabel = (page: number, pageSize: number, length: number) => {
		if (length === 0 || pageSize === 0) {
			return `0 ${this.ofLabel} ${length}`;
		}
		length = Math.max(length, 0);
		const startIndex = page * pageSize;
		const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;
		return `${startIndex + 1} - ${endIndex} ${this.ofLabel} ${length}`;
		// tslint:disable-next-line: semicolon
	};
}
