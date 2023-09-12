import { Injectable } from "@angular/core";
import * as FileSaver from "file-saver";
import * as XLSX from "xlsx";

import { TranslatePipe } from "../pipes/translate.pipe";

const EXCEL_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8";
const EXCEL_EXTENSION = ".xls";

@Injectable()
export class ExcelService {
	POSITION_HEADER = ["A1", "B1", "C1", "D1", "E1", "F1", "G1", "H1", "I1", "J1", "K1", "L1", "M1"];
	constructor(private translatePipe: TranslatePipe) {}

	public exportAsExcelFile(json: any[], excelFileName: string, columnsToDelete: string[] = []): void {
		this.deleteProperties(json, columnsToDelete);
		const columnNamesTranslated = this.translateColumnNamesExcel(json, columnsToDelete);

		const myworksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
		columnNamesTranslated.forEach((headerValue, index) => {
			myworksheet[this.POSITION_HEADER[index]].v = headerValue;
		});

		const myworkbook: XLSX.WorkBook = { Sheets: { data: myworksheet }, SheetNames: ["data"] };
		const excelBuffer: any = XLSX.write(myworkbook, { bookType: "xls", type: "array" });
		this.saveAsExcelFile(excelBuffer, excelFileName);
	}

	private translateColumnNamesExcel(json: any[], columnsDelete: string[] = []): string[] {
		const arrayColumns = [];
		const objectNames = Object.keys(json[0]);

		objectNames.forEach((value) => {
			if (!columnsDelete.includes(value)) {
				arrayColumns.push(this.translatePipe.transform(value));
			}
		});

		return arrayColumns;
	}

	private deleteProperties(json: any[], columnsToDelete: string[] = []): void {
		return json.forEach((value) => columnsToDelete.forEach((column) => delete value[column]));
	}

	private saveAsExcelFile(buffer: any, fileName: string): void {
		const data: Blob = new Blob([buffer], {
			type: EXCEL_TYPE
		});
		FileSaver.saveAs(data, `${fileName}_exported${EXCEL_EXTENSION}`);
	}
}
